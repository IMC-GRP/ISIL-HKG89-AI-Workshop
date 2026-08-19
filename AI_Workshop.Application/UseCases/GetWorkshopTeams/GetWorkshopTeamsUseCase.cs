using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetWorkshopTeams;

public class GetWorkshopTeamsUseCase
{
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;
    private readonly EnsureFinalistTeamsUseCase _ensureFinalistTeamsUseCase;

    public GetWorkshopTeamsUseCase(
        IWorkshopTeamRepository workshopTeamRepository,
        IIdeaRepository ideaRepository,
        IWorkshopSettingsRepository workshopSettingsRepository,
        EnsureFinalistTeamsUseCase ensureFinalistTeamsUseCase)
    {
        _workshopTeamRepository = workshopTeamRepository;
        _ideaRepository = ideaRepository;
        _workshopSettingsRepository = workshopSettingsRepository;
        _ensureFinalistTeamsUseCase = ensureFinalistTeamsUseCase;
    }

    public async Task<IReadOnlyCollection<WorkshopTeamDto>> ExecuteAsync()
    {
        await _ensureFinalistTeamsUseCase.EnsureFinalistTeamsAsync();

        var ideas = await _ideaRepository.GetAllIdeasAsync();
        var finalistIdeaIds = ideas
            .Where(idea => idea.Status == Domain.Enums.IdeaStatus.Selected)
            .Select(idea => idea.Id)
            .ToHashSet();

        var teams = (await _workshopTeamRepository.GetAllTeamsAsync())
            .Where(team => finalistIdeaIds.Contains(team.IdeaId))
            .ToArray();

        var ideaLookup = ideas.ToDictionary(idea => idea.Id);
        var settings = await _workshopSettingsRepository.GetSettingsAsync();

        return teams
            .Select(team => WorkshopTeamDtoMapper.Map(team, ideaLookup.TryGetValue(team.IdeaId, out var idea) ? idea : null, settings))
            .ToArray();
    }
}
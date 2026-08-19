using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Enums;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetWorkshopTeamById;

public class GetWorkshopTeamByIdUseCase
{
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;
    private readonly EnsureFinalistTeamsUseCase _ensureFinalistTeamsUseCase;

    public GetWorkshopTeamByIdUseCase(
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

    public async Task<WorkshopTeamDto?> ExecuteAsync(int id)
    {
        await _ensureFinalistTeamsUseCase.EnsureFinalistTeamsAsync();

        var team = await _workshopTeamRepository.GetTeamByIdAsync(id);
        if (team is null)
        {
            return null;
        }

        var idea = await _ideaRepository.GetIdeaByIdAsync(team.IdeaId);
        if (idea?.Status != IdeaStatus.Selected)
        {
            return null;
        }

        var settings = await _workshopSettingsRepository.GetSettingsAsync();
        return WorkshopTeamDtoMapper.Map(team, idea, settings);
    }
}
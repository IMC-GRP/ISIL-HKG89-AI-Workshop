using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Enums;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetWorkshopTeamByIdeaId;

public class GetWorkshopTeamByIdeaIdUseCase
{
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;
    private readonly EnsureFinalistTeamsUseCase _ensureFinalistTeamsUseCase;

    public GetWorkshopTeamByIdeaIdUseCase(
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

    public async Task<WorkshopTeamDto?> ExecuteAsync(int ideaId)
    {
        await _ensureFinalistTeamsUseCase.EnsureFinalistTeamsAsync();

        var idea = await _ideaRepository.GetIdeaByIdAsync(ideaId);
        if (idea?.Status != IdeaStatus.Selected)
        {
            return null;
        }

        var team = await _workshopTeamRepository.GetTeamByIdeaIdAsync(ideaId);
        if (team is null)
        {
            return null;
        }

        var settings = await _workshopSettingsRepository.GetSettingsAsync();
        return WorkshopTeamDtoMapper.Map(team, idea, settings);
    }
}
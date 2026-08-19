using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetEmployeeWorkshopTeam;

public class GetEmployeeWorkshopTeamUseCase
{
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;

    public GetEmployeeWorkshopTeamUseCase(
        IWorkshopTeamRepository workshopTeamRepository,
        IIdeaRepository ideaRepository,
        IWorkshopSettingsRepository workshopSettingsRepository)
    {
        _workshopTeamRepository = workshopTeamRepository;
        _ideaRepository = ideaRepository;
        _workshopSettingsRepository = workshopSettingsRepository;
    }

    public async Task<WorkshopTeamDto?> ExecuteAsync(string employeeEmail)
    {
        var normalizedEmail = WorkshopTeamBusinessRules.NormalizeEmail(employeeEmail);
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return null;
        }

        var team = await _workshopTeamRepository.GetEmployeeTeamAsync(normalizedEmail);
        if (team is null)
        {
            return null;
        }

        var idea = await _ideaRepository.GetIdeaByIdAsync(team.IdeaId);
        var settings = await _workshopSettingsRepository.GetSettingsAsync();
        return WorkshopTeamDtoMapper.Map(team, idea, settings);
    }
}
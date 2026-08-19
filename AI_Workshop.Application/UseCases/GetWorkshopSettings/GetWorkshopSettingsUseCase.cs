using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetWorkshopSettings;

public class GetWorkshopSettingsUseCase
{
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;

    public GetWorkshopSettingsUseCase(IWorkshopSettingsRepository workshopSettingsRepository)
    {
        _workshopSettingsRepository = workshopSettingsRepository;
    }

    public async Task<WorkshopSettingsDto> ExecuteAsync()
    {
        var settings = await _workshopSettingsRepository.GetSettingsAsync();
        return new WorkshopSettingsDto
        {
            IsTeamRegistrationOpen = settings.IsTeamRegistrationOpen,
            TeamRegistrationCloseDate = settings.TeamRegistrationCloseDate,
            IsTeamRegistrationCurrentlyOpen = WorkshopTeamBusinessRules.IsWorkshopRegistrationOpen(settings, DateTime.UtcNow)
        };
    }
}
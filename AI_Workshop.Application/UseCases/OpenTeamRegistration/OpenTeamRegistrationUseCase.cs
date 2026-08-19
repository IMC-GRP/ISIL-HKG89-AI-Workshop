using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.OpenTeamRegistration;

public class OpenTeamRegistrationUseCase
{
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;
    private readonly IWorkshopTeamRepository _workshopTeamRepository;

    public OpenTeamRegistrationUseCase(IWorkshopSettingsRepository workshopSettingsRepository, IWorkshopTeamRepository workshopTeamRepository)
    {
        _workshopSettingsRepository = workshopSettingsRepository;
        _workshopTeamRepository = workshopTeamRepository;
    }

    public async Task<WorkshopSettingsDto> ExecuteAsync()
    {
        var settings = await _workshopSettingsRepository.SetTeamRegistrationOpenAsync(true);
        await _workshopTeamRepository.SetTeamRegistrationStateAsync(true, settings.TeamRegistrationCloseDate);

        return new WorkshopSettingsDto
        {
            IsTeamRegistrationOpen = settings.IsTeamRegistrationOpen,
            TeamRegistrationCloseDate = settings.TeamRegistrationCloseDate,
            IsTeamRegistrationCurrentlyOpen = WorkshopTeamBusinessRules.IsWorkshopRegistrationOpen(settings, DateTime.UtcNow)
        };
    }
}
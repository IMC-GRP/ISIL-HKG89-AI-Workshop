using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.UpdateTeamRegistrationCloseDate;

public class UpdateTeamRegistrationCloseDateUseCase
{
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;
    private readonly IWorkshopTeamRepository _workshopTeamRepository;

    public UpdateTeamRegistrationCloseDateUseCase(IWorkshopSettingsRepository workshopSettingsRepository, IWorkshopTeamRepository workshopTeamRepository)
    {
        _workshopSettingsRepository = workshopSettingsRepository;
        _workshopTeamRepository = workshopTeamRepository;
    }

    public async Task<WorkshopSettingsDto> ExecuteAsync(UpdateTeamRegistrationCloseDateRequestDto request)
    {
        var settings = await _workshopSettingsRepository.SetTeamRegistrationCloseDateAsync(request.TeamRegistrationCloseDate);
        await _workshopTeamRepository.SetTeamRegistrationStateAsync(settings.IsTeamRegistrationOpen, settings.TeamRegistrationCloseDate);

        return new WorkshopSettingsDto
        {
            IsTeamRegistrationOpen = settings.IsTeamRegistrationOpen,
            TeamRegistrationCloseDate = settings.TeamRegistrationCloseDate,
            IsTeamRegistrationCurrentlyOpen = WorkshopTeamBusinessRules.IsWorkshopRegistrationOpen(settings, DateTime.UtcNow)
        };
    }
}
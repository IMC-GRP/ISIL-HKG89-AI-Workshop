using AI_Workshop.Domain.Entities;

namespace AI_Workshop.Domain.Interfaces;

public interface IWorkshopSettingsRepository
{
    Task<WorkshopSettings> GetSettingsAsync();
    Task<WorkshopSettings> SetTeamRegistrationOpenAsync(bool isOpen);
    Task<WorkshopSettings> SetTeamRegistrationCloseDateAsync(DateTime? closeDate);
}
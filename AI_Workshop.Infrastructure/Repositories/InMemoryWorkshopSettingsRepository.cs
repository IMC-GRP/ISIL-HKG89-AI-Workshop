using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Infrastructure.Repositories;

public class InMemoryWorkshopSettingsRepository : IWorkshopSettingsRepository
{
    private readonly object _syncLock = new();
    private WorkshopSettings _settings = new()
    {
        IsTeamRegistrationOpen = true,
        TeamRegistrationCloseDate = DateTime.UtcNow.AddDays(10)
    };

    public Task<WorkshopSettings> GetSettingsAsync()
    {
        lock (_syncLock)
        {
            return Task.FromResult(Clone(_settings));
        }
    }

    public Task<WorkshopSettings> SetTeamRegistrationOpenAsync(bool isOpen)
    {
        lock (_syncLock)
        {
            _settings.IsTeamRegistrationOpen = isOpen;
            return Task.FromResult(Clone(_settings));
        }
    }

    public Task<WorkshopSettings> SetTeamRegistrationCloseDateAsync(DateTime? closeDate)
    {
        lock (_syncLock)
        {
            _settings.TeamRegistrationCloseDate = closeDate;
            return Task.FromResult(Clone(_settings));
        }
    }

    private static WorkshopSettings Clone(WorkshopSettings settings)
    {
        return new WorkshopSettings
        {
            IsTeamRegistrationOpen = settings.IsTeamRegistrationOpen,
            TeamRegistrationCloseDate = settings.TeamRegistrationCloseDate
        };
    }
}
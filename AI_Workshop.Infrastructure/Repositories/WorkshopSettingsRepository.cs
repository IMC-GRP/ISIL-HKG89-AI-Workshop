using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Interfaces;
using AI_Workshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AI_Workshop.Infrastructure.Repositories;

public class WorkshopSettingsRepository : IWorkshopSettingsRepository
{
    private readonly AIWorkshopDbContext _dbContext;

    public WorkshopSettingsRepository(AIWorkshopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkshopSettings> GetSettingsAsync()
    {
        var settings = await _dbContext.WorkshopSettings
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return settings ?? throw new InvalidOperationException("No row found in dbo.WorkshopSettings.");
    }

    public async Task<WorkshopSettings> SetTeamRegistrationOpenAsync(bool isOpen)
    {
        var settings = await GetTrackedSettingsAsync();
        settings.IsTeamRegistrationOpen = isOpen;

        await _dbContext.SaveChangesAsync();
        return Clone(settings);
    }

    public async Task<WorkshopSettings> SetTeamRegistrationCloseDateAsync(DateTime? closeDate)
    {
        var settings = await GetTrackedSettingsAsync();
        settings.TeamRegistrationCloseDate = closeDate;

        await _dbContext.SaveChangesAsync();
        return Clone(settings);
    }

    private async Task<WorkshopSettings> GetTrackedSettingsAsync()
    {
        var settings = await _dbContext.WorkshopSettings.FirstOrDefaultAsync();
        return settings ?? throw new InvalidOperationException("No row found in dbo.WorkshopSettings.");
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

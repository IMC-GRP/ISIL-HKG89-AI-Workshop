using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Interfaces;
using AI_Workshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AI_Workshop.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AIWorkshopDbContext _dbContext;

    public ScheduleRepository(AIWorkshopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ScheduleItem>> GetCompleteScheduleAsync()
    {
        return await _dbContext.WorkshopScheduleItems
            .AsNoTracking()
            .OrderBy(item => item.DayNumber)
            .ThenBy(item => item.DisplayOrder)
            .ToArrayAsync();
    }

    public async Task<IReadOnlyCollection<ScheduleItem>> GetScheduleByDayAsync(int dayNumber)
    {
        return await _dbContext.WorkshopScheduleItems
            .AsNoTracking()
            .Where(item => item.DayNumber == dayNumber)
            .OrderBy(item => item.DisplayOrder)
            .ToArrayAsync();
    }
}

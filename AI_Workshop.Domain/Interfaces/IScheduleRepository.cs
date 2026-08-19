using AI_Workshop.Domain.Entities;

namespace AI_Workshop.Domain.Interfaces;

public interface IScheduleRepository
{
    Task<IReadOnlyCollection<ScheduleItem>> GetCompleteScheduleAsync();
    Task<IReadOnlyCollection<ScheduleItem>> GetScheduleByDayAsync(int dayNumber);
}

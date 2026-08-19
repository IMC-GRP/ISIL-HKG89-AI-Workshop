using AI_Workshop.Application.DTOs;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetSchedule;

public class GetScheduleByDayUseCase
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetScheduleByDayUseCase(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<IReadOnlyCollection<ScheduleItemDto>> ExecuteAsync(int dayNumber)
    {
        var scheduleItems = await _scheduleRepository.GetScheduleByDayAsync(dayNumber);

        return scheduleItems
            .Select(item => new ScheduleItemDto
            {
                Id = item.Id,
                DayNumber = item.DayNumber,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                Type = item.Type,
                Title = item.Title,
                Description = item.Description,
                Location = item.Location,
                DisplayOrder = item.DisplayOrder
            })
            .ToArray();
    }
}

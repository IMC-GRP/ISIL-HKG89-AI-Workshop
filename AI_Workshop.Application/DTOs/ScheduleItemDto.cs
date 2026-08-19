using AI_Workshop.Domain.Entities;

namespace AI_Workshop.Application.DTOs;

public class ScheduleItemDto
{
    public int Id { get; set; }
    public int DayNumber { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public ScheduleItemType Type { get; set; } = ScheduleItemType.General;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int DisplayOrder { get; set; }
}

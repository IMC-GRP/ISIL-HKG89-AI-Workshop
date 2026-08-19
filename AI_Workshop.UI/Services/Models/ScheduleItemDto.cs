namespace AI_Workshop.UI.Services.Models;

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

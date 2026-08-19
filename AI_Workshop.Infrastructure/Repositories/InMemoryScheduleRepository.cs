using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Infrastructure.Repositories;

public class InMemoryScheduleRepository : IScheduleRepository
{
    private readonly IReadOnlyCollection<ScheduleItem> _scheduleItems;

    public InMemoryScheduleRepository()
    {
        _scheduleItems = new List<ScheduleItem>
        {
            new()
            {
                Id = 1,
                DayNumber = 1,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),
                Type = ScheduleItemType.Opening,
                Title = "Opening & Kickoff",
                Description = "Welcome, workshop overview, goals, team introductions and expectations.",
                Location = "Main Conference Room",
                DisplayOrder = 1
            },
            new()
            {
                Id = 2,
                DayNumber = 1,
                StartTime = new TimeSpan(9, 30, 0),
                EndTime = new TimeSpan(10, 30, 0),
                Type = ScheduleItemType.Workshop,
                Title = "AI Inspiration Session",
                Description = "Practical AI capabilities, examples and tools to inspire the teams.",
                Location = "Innovation Room",
                DisplayOrder = 2
            },
            new()
            {
                Id = 3,
                DayNumber = 1,
                StartTime = new TimeSpan(10, 30, 0),
                EndTime = new TimeSpan(10, 45, 0),
                Type = ScheduleItemType.Break,
                Title = "Coffee Break",
                Location = "Team Area",
                DisplayOrder = 3
            },
            new()
            {
                Id = 4,
                DayNumber = 1,
                StartTime = new TimeSpan(10, 45, 0),
                EndTime = new TimeSpan(12, 30, 0),
                Type = ScheduleItemType.TeamWork,
                Title = "Team Work — Discovery & Planning",
                Description = "Define the problem, users, solution direction and initial implementation plan.",
                Location = "Team Area",
                DisplayOrder = 4
            },
            new()
            {
                Id = 5,
                DayNumber = 1,
                StartTime = new TimeSpan(12, 30, 0),
                EndTime = new TimeSpan(13, 15, 0),
                Type = ScheduleItemType.Meal,
                Title = "Lunch",
                Location = "Main Conference Room",
                DisplayOrder = 5
            },
            new()
            {
                Id = 6,
                DayNumber = 1,
                StartTime = new TimeSpan(13, 15, 0),
                EndTime = new TimeSpan(14, 0, 0),
                Type = ScheduleItemType.Mentoring,
                Title = "Mentoring Session",
                Description = "Teams meet with mentors to validate their direction and identify challenges.",
                Location = "Innovation Room",
                DisplayOrder = 6
            },
            new()
            {
                Id = 7,
                DayNumber = 1,
                StartTime = new TimeSpan(14, 0, 0),
                EndTime = new TimeSpan(16, 0, 0),
                Type = ScheduleItemType.TeamWork,
                Title = "Build Session",
                Description = "Start building the solution and turn the concept into a working prototype.",
                Location = "Team Area",
                DisplayOrder = 7
            },
            new()
            {
                Id = 8,
                DayNumber = 1,
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(16, 15, 0),
                Type = ScheduleItemType.Closing,
                Title = "Day 1 Wrap-Up",
                Description = "Quick team updates and preparation for Day 2.",
                Location = "Main Conference Room",
                DisplayOrder = 8
            },
            new()
            {
                Id = 9,
                DayNumber = 2,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 15, 0),
                Type = ScheduleItemType.Opening,
                Title = "Day 2 Kickoff",
                Location = "Innovation Lab",
                DisplayOrder = 1
            },
            new()
            {
                Id = 10,
                DayNumber = 2,
                StartTime = new TimeSpan(9, 15, 0),
                EndTime = new TimeSpan(11, 0, 0),
                Type = ScheduleItemType.TeamWork,
                Title = "Build & Iterate",
                Description = "Continue development, test assumptions and improve the solution.",
                Location = "Team Area",
                DisplayOrder = 2
            },
            new()
            {
                Id = 11,
                DayNumber = 2,
                StartTime = new TimeSpan(11, 0, 0),
                EndTime = new TimeSpan(11, 15, 0),
                Type = ScheduleItemType.Break,
                Title = "Coffee Break",
                Location = "Team Area",
                DisplayOrder = 3
            },
            new()
            {
                Id = 12,
                DayNumber = 2,
                StartTime = new TimeSpan(11, 15, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Type = ScheduleItemType.Mentoring,
                Title = "Mentoring Checkpoint",
                Description = "Final mentor feedback before moving into presentation preparation.",
                Location = "Innovation Room",
                DisplayOrder = 4
            },
            new()
            {
                Id = 13,
                DayNumber = 2,
                StartTime = new TimeSpan(12, 0, 0),
                EndTime = new TimeSpan(12, 45, 0),
                Type = ScheduleItemType.Meal,
                Title = "Lunch",
                Location = "Main Conference Room",
                DisplayOrder = 5
            },
            new()
            {
                Id = 14,
                DayNumber = 2,
                StartTime = new TimeSpan(12, 45, 0),
                EndTime = new TimeSpan(14, 0, 0),
                Type = ScheduleItemType.TeamWork,
                Title = "Final Build & Polish",
                Description = "Complete the prototype, validate the demo and prepare the final story.",
                Location = "Team Area",
                DisplayOrder = 6
            },
            new()
            {
                Id = 15,
                DayNumber = 2,
                StartTime = new TimeSpan(14, 0, 0),
                EndTime = new TimeSpan(14, 45, 0),
                Type = ScheduleItemType.Presentation,
                Title = "Presentation Preparation",
                Description = "Prepare the final pitch, demo and key impact message.",
                Location = "Innovation Room",
                DisplayOrder = 7
            },
            new()
            {
                Id = 16,
                DayNumber = 2,
                StartTime = new TimeSpan(14, 45, 0),
                EndTime = new TimeSpan(16, 0, 0),
                Type = ScheduleItemType.Presentation,
                Title = "Final Presentations",
                Description = "The four finalist teams present their solutions.",
                Location = "Auditorium",
                DisplayOrder = 8
            },
            new()
            {
                Id = 17,
                DayNumber = 2,
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(16, 30, 0),
                Type = ScheduleItemType.Closing,
                Title = "Closing",
                Description = "Workshop summary and closing session.",
                Location = "Main Conference Room",
                DisplayOrder = 9
            }
        };
    }

    public Task<IReadOnlyCollection<ScheduleItem>> GetCompleteScheduleAsync()
    {
        var items = _scheduleItems
            .OrderBy(item => item.DayNumber)
            .ThenBy(item => item.DisplayOrder)
            .Select(CloneScheduleItem)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<ScheduleItem>>(items);
    }

    public Task<IReadOnlyCollection<ScheduleItem>> GetScheduleByDayAsync(int dayNumber)
    {
        var items = _scheduleItems
            .Where(item => item.DayNumber == dayNumber)
            .OrderBy(item => item.DisplayOrder)
            .Select(CloneScheduleItem)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<ScheduleItem>>(items);
    }

    private static ScheduleItem CloneScheduleItem(ScheduleItem item)
    {
        return new ScheduleItem
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
        };
    }
}

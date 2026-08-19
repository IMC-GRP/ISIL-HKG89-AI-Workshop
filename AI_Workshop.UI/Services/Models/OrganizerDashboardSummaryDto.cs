namespace AI_Workshop.UI.Services.Models;

public class OrganizerDashboardSummaryDto
{
    public int TotalIdeas { get; set; }
    public int SubmittedIdeas { get; set; }
    public int FinalistIdeas { get; set; }
    public int RegisteredParticipants { get; set; }
    public int AvailableTeamSpots { get; set; }
    public WorkshopSettingsDto Registration { get; set; } = new();
    public IReadOnlyCollection<OrganizerIdeaStatusCountDto> IdeaStatusCounts { get; set; } = [];
    public IReadOnlyCollection<OrganizerIdeaCategoryCountDto> IdeaCategoryCounts { get; set; } = [];
    public IReadOnlyCollection<OrganizerMembershipConflictDto> MembershipConflicts { get; set; } = [];
    public OrganizerReadinessDto Readiness { get; set; } = new();
}

public class OrganizerIdeaStatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class OrganizerIdeaCategoryCountDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class OrganizerMembershipConflictDto
{
    public string EmployeeEmail { get; set; } = string.Empty;
    public IReadOnlyCollection<string> TeamNames { get; set; } = [];
}

public class OrganizerReadinessDto
{
    public bool HasFourFinalists { get; set; }
    public bool HasFourTeams { get; set; }
    public bool EachFinalistHasTeamLeader { get; set; }
    public bool HasDayOneSchedule { get; set; }
    public bool HasDayTwoSchedule { get; set; }
    public bool IsRegistrationStillOpen { get; set; }
}

namespace AI_Workshop.UI.Services.Models;

public class WorkshopTeamDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public string IdeaTitle { get; set; } = string.Empty;
    public IdeaCategory IdeaCategory { get; set; }
    public string IdeaDescription { get; set; } = string.Empty;
    public string TeamLeaderName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int MaxMembers { get; set; }
    public int CurrentMemberCount { get; set; }
    public int AvailableSpots { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public DateTime? RegistrationCloseDate { get; set; }
    public bool IsRegistrationCurrentlyOpen { get; set; }
    public IReadOnlyCollection<WorkshopTeamMemberDto> TeamMembers { get; set; } = [];
}
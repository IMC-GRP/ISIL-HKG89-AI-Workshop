namespace AI_Workshop.Domain.Entities;

public class WorkshopTeam
{
    public const int DefaultMaxMembers = 10;

    public int Id { get; set; }
    public int IdeaId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int MaxMembers { get; set; } = DefaultMaxMembers;
    public DateTime CreatedDate { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public DateTime? RegistrationCloseDate { get; set; }
    public List<WorkshopTeamMember> Members { get; set; } = [];
}
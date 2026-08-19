namespace AI_Workshop.Domain.Entities;

public class WorkshopTeamMember
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeEmail { get; set; }
    public DateTime JoinedDate { get; set; }
    public bool IsTeamLeader { get; set; }
}
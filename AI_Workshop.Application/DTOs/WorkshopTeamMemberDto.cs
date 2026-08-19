namespace AI_Workshop.Application.DTOs;

public class WorkshopTeamMemberDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeEmail { get; set; }
    public DateTime JoinedDate { get; set; }
    public bool IsTeamLeader { get; set; }
}
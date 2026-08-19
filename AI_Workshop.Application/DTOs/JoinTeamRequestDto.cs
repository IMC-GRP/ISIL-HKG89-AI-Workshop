namespace AI_Workshop.Application.DTOs;

public class JoinTeamRequestDto
{
    public int TeamId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
}
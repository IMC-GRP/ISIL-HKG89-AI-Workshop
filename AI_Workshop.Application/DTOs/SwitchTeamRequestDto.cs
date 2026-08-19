namespace AI_Workshop.Application.DTOs;

public class SwitchTeamRequestDto
{
    public int TargetTeamId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
}
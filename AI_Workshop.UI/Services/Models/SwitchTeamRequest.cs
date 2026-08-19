namespace AI_Workshop.UI.Services.Models;

public class SwitchTeamRequest
{
    public int TargetTeamId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
}

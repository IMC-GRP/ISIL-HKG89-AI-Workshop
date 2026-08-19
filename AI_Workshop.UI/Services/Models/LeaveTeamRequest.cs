namespace AI_Workshop.UI.Services.Models;

public class LeaveTeamRequest
{
    public int TeamId { get; set; }
    public string EmployeeEmail { get; set; } = string.Empty;
}

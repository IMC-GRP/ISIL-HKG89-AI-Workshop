namespace AI_Workshop.Application.DTOs;

public class TeamMembershipActionResultDto
{
    public bool Succeeded { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public bool RequiresConfirmation { get; set; }
}
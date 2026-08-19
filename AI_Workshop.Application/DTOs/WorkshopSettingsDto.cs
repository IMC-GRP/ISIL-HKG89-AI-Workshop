namespace AI_Workshop.Application.DTOs;

public class WorkshopSettingsDto
{
    public bool IsTeamRegistrationOpen { get; set; }
    public DateTime? TeamRegistrationCloseDate { get; set; }
    public bool IsTeamRegistrationCurrentlyOpen { get; set; }
}
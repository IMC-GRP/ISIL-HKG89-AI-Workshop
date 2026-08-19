namespace AI_Workshop.UI.Services.Models;

public class WorkshopSettingsDto
{
    public bool IsTeamRegistrationOpen { get; set; }
    public DateTime? TeamRegistrationCloseDate { get; set; }
    public bool IsTeamRegistrationCurrentlyOpen { get; set; }
}

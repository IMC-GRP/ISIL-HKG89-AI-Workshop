namespace AI_Workshop.Domain.Entities;

public class WorkshopSettings
{
    public bool IsTeamRegistrationOpen { get; set; }
    public DateTime? TeamRegistrationCloseDate { get; set; }
}
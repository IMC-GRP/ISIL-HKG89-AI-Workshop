using AI_Workshop.Domain.Entities;

namespace AI_Workshop.Application.UseCases.WorkshopTeams;

internal static class WorkshopTeamBusinessRules
{
    public static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public static bool IsWorkshopRegistrationOpen(WorkshopSettings settings, DateTime utcNow)
    {
        return settings.IsTeamRegistrationOpen
               && (!settings.TeamRegistrationCloseDate.HasValue || utcNow <= settings.TeamRegistrationCloseDate.Value);
    }

    public static bool IsTeamRegistrationAllowed(WorkshopSettings settings, WorkshopTeam team, DateTime utcNow)
    {
        return IsWorkshopRegistrationOpen(settings, utcNow)
               && team.IsRegistrationOpen
               && (!team.RegistrationCloseDate.HasValue || utcNow <= team.RegistrationCloseDate.Value);
    }
}
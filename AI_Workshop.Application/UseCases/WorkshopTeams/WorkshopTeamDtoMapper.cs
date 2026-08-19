using AI_Workshop.Application.DTOs;
using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Enums;

namespace AI_Workshop.Application.UseCases.WorkshopTeams;

internal static class WorkshopTeamDtoMapper
{
    public static WorkshopTeamDto Map(WorkshopTeam team, Idea? idea, WorkshopSettings? settings)
    {
        var members = team.Members
            .OrderByDescending(member => member.IsTeamLeader)
            .ThenBy(member => member.JoinedDate)
            .ThenBy(member => member.EmployeeName)
            .Select(MapMember)
            .ToArray();

        var registrationSettings = settings ?? new WorkshopSettings
        {
            IsTeamRegistrationOpen = team.IsRegistrationOpen,
            TeamRegistrationCloseDate = team.RegistrationCloseDate
        };

        return new WorkshopTeamDto
        {
            Id = team.Id,
            IdeaId = team.IdeaId,
            IdeaTitle = idea?.Title ?? string.Empty,
            IdeaCategory = idea?.Category ?? IdeaCategory.Other,
            IdeaDescription = idea?.Description ?? string.Empty,
            TeamLeaderName = members.FirstOrDefault(member => member.IsTeamLeader)?.EmployeeName ?? idea?.TeamLeaderName ?? string.Empty,
            TeamName = team.TeamName,
            MaxMembers = team.MaxMembers,
            CurrentMemberCount = members.Length,
            AvailableSpots = Math.Max(0, team.MaxMembers - members.Length),
            IsRegistrationOpen = team.IsRegistrationOpen,
            RegistrationCloseDate = team.RegistrationCloseDate,
            IsRegistrationCurrentlyOpen = WorkshopTeamBusinessRules.IsTeamRegistrationAllowed(registrationSettings, team, DateTime.UtcNow),
            TeamMembers = members
        };
    }

    private static WorkshopTeamMemberDto MapMember(WorkshopTeamMember member)
    {
        return new WorkshopTeamMemberDto
        {
            Id = member.Id,
            TeamId = member.TeamId,
            EmployeeName = member.EmployeeName,
            EmployeeEmail = member.EmployeeEmail,
            JoinedDate = member.JoinedDate,
            IsTeamLeader = member.IsTeamLeader
        };
    }
}
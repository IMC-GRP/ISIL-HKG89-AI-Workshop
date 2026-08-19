using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.LeaveWorkshopTeam;

public class LeaveWorkshopTeamUseCase
{
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;

    public LeaveWorkshopTeamUseCase(IWorkshopTeamRepository workshopTeamRepository, IWorkshopSettingsRepository workshopSettingsRepository)
    {
        _workshopTeamRepository = workshopTeamRepository;
        _workshopSettingsRepository = workshopSettingsRepository;
    }

    public async Task<TeamMembershipActionResultDto> ExecuteAsync(int teamId, string employeeEmail)
    {
        var email = WorkshopTeamBusinessRules.NormalizeEmail(employeeEmail);
        if (string.IsNullOrWhiteSpace(email))
        {
            return Failed("validation_failed", "Email is required.");
        }

        var team = await _workshopTeamRepository.GetTeamByIdAsync(teamId);
        if (team is null)
        {
            return Failed("team_not_found", "Team not found.");
        }

        var settings = await _workshopSettingsRepository.GetSettingsAsync();
        if (!WorkshopTeamBusinessRules.IsTeamRegistrationAllowed(settings, team, DateTime.UtcNow))
        {
            return Failed("registration_closed", "Team registration is closed.");
        }

        var member = team.Members.FirstOrDefault(m => WorkshopTeamBusinessRules.NormalizeEmail(m.EmployeeEmail) == email);
        if (member is null)
        {
            return Failed("member_not_found", "You are not a member of this team.");
        }

        if (member.IsTeamLeader)
        {
            return Failed("leader_cannot_leave", "Team leaders cannot leave their finalist team.");
        }

        await _workshopTeamRepository.LeaveTeamAsync(teamId, email);
        return new TeamMembershipActionResultDto
        {
            Succeeded = true,
            Code = "left_team",
            Message = "You have left the team.",
            TeamId = team.Id,
            TeamName = team.TeamName
        };
    }

    private static TeamMembershipActionResultDto Failed(string code, string message) =>
        new()
        {
            Succeeded = false,
            Code = code,
            Message = message
        };
}
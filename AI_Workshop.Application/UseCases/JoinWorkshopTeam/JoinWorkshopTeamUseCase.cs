using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.JoinWorkshopTeam;

public class JoinWorkshopTeamUseCase
{
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;

    public JoinWorkshopTeamUseCase(IWorkshopTeamRepository workshopTeamRepository, IWorkshopSettingsRepository workshopSettingsRepository)
    {
        _workshopTeamRepository = workshopTeamRepository;
        _workshopSettingsRepository = workshopSettingsRepository;
    }

    public async Task<TeamMembershipActionResultDto> ExecuteAsync(JoinTeamRequestDto request)
    {
        var email = WorkshopTeamBusinessRules.NormalizeEmail(request.EmployeeEmail);
        if (string.IsNullOrWhiteSpace(request.EmployeeName) || string.IsNullOrWhiteSpace(email))
        {
            return Failed("validation_failed", "Name and email are required.");
        }

        var team = await _workshopTeamRepository.GetTeamByIdAsync(request.TeamId);
        if (team is null)
        {
            return Failed("team_not_found", "Team not found.");
        }

        var settings = await _workshopSettingsRepository.GetSettingsAsync();
        if (!WorkshopTeamBusinessRules.IsTeamRegistrationAllowed(settings, team, DateTime.UtcNow))
        {
            return Failed("registration_closed", "Team registration is closed.");
        }

        if (team.Members.Count >= team.MaxMembers)
        {
            return Failed("team_full", "This team is full.");
        }

        var currentTeam = await _workshopTeamRepository.GetEmployeeTeamAsync(email);
        if (currentTeam is not null && currentTeam.Id == team.Id)
        {
            return new TeamMembershipActionResultDto
            {
                Succeeded = true,
                Code = "already_member",
                Message = "You are already a member of this team.",
                TeamId = team.Id,
                TeamName = team.TeamName
            };
        }

        if (currentTeam is not null)
        {
            var currentMember = currentTeam.Members
                .FirstOrDefault(member => WorkshopTeamBusinessRules.NormalizeEmail(member.EmployeeEmail) == email);

            if (currentMember?.IsTeamLeader == true)
            {
                return Failed("leader_cannot_switch", "Team leaders cannot switch teams.");
            }

            return new TeamMembershipActionResultDto
            {
                Succeeded = false,
                Code = "already_in_other_team",
                Message = $"You're currently a member of \"{currentTeam.TeamName}\".",
                TeamId = currentTeam.Id,
                TeamName = currentTeam.TeamName,
                RequiresConfirmation = true
            };
        }

        await _workshopTeamRepository.JoinTeamAsync(team.Id, request.EmployeeName.Trim(), email);

        return new TeamMembershipActionResultDto
        {
            Succeeded = true,
            Code = "joined",
            Message = "Successfully joined team.",
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
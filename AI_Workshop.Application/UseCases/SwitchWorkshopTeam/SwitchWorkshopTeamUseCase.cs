using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.SwitchWorkshopTeam;

public class SwitchWorkshopTeamUseCase
{
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;

    public SwitchWorkshopTeamUseCase(IWorkshopTeamRepository workshopTeamRepository, IWorkshopSettingsRepository workshopSettingsRepository)
    {
        _workshopTeamRepository = workshopTeamRepository;
        _workshopSettingsRepository = workshopSettingsRepository;
    }

    public async Task<TeamMembershipActionResultDto> ExecuteAsync(SwitchTeamRequestDto request)
    {
        var email = WorkshopTeamBusinessRules.NormalizeEmail(request.EmployeeEmail);
        if (string.IsNullOrWhiteSpace(request.EmployeeName) || string.IsNullOrWhiteSpace(email))
        {
            return Failed("validation_failed", "Name and email are required.");
        }

        var currentTeam = await _workshopTeamRepository.GetEmployeeTeamAsync(email);
        if (currentTeam is null)
        {
            return Failed("no_current_team", "You are not currently a member of any finalist team.");
        }

        var currentMember = currentTeam.Members.FirstOrDefault(member => WorkshopTeamBusinessRules.NormalizeEmail(member.EmployeeEmail) == email);
        if (currentMember?.IsTeamLeader == true)
        {
            return Failed("leader_cannot_switch", "Team leaders cannot switch teams.");
        }

        var targetTeam = await _workshopTeamRepository.GetTeamByIdAsync(request.TargetTeamId);
        if (targetTeam is null)
        {
            return Failed("target_team_not_found", "Target team not found.");
        }

        if (targetTeam.Id == currentTeam.Id)
        {
            return new TeamMembershipActionResultDto
            {
                Succeeded = true,
                Code = "already_member",
                Message = "You are already a member of this team.",
                TeamId = currentTeam.Id,
                TeamName = currentTeam.TeamName
            };
        }

        var settings = await _workshopSettingsRepository.GetSettingsAsync();
        if (!WorkshopTeamBusinessRules.IsTeamRegistrationAllowed(settings, currentTeam, DateTime.UtcNow)
            || !WorkshopTeamBusinessRules.IsTeamRegistrationAllowed(settings, targetTeam, DateTime.UtcNow))
        {
            return Failed("registration_closed", "Team registration is closed.");
        }

        if (targetTeam.Members.Count >= targetTeam.MaxMembers)
        {
            return Failed("team_full", "This team is full.");
        }

        var switched = await _workshopTeamRepository.SwitchTeamAsync(email, request.EmployeeName.Trim(), targetTeam.Id);
        if (!switched)
        {
            return Failed("switch_failed", "Unable to move to the target team. You remain in your current team.");
        }

        return new TeamMembershipActionResultDto
        {
            Succeeded = true,
            Code = "switched",
            Message = "You successfully moved to another team.",
            TeamId = targetTeam.Id,
            TeamName = targetTeam.TeamName
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
using AI_Workshop.Domain.Entities;

namespace AI_Workshop.Domain.Interfaces;

public interface IWorkshopTeamRepository
{
    Task<IReadOnlyCollection<WorkshopTeam>> GetAllTeamsAsync();
    Task<WorkshopTeam?> GetTeamByIdAsync(int id);
    Task<WorkshopTeam?> GetTeamByIdeaIdAsync(int ideaId);
    Task<WorkshopTeam> CreateTeamAsync(WorkshopTeam team);
    Task<IReadOnlyCollection<WorkshopTeamMember>> GetTeamMembersAsync(int teamId);
    Task<WorkshopTeam?> GetEmployeeTeamAsync(string employeeEmail);
    Task<WorkshopTeamMember?> EnsureTeamLeaderMemberAsync(int teamId, string employeeName, string employeeEmail);
    Task<WorkshopTeamMember?> JoinTeamAsync(int teamId, string employeeName, string employeeEmail);
    Task<bool> LeaveTeamAsync(int teamId, string employeeEmail);
    Task<bool> SwitchTeamAsync(string employeeEmail, string employeeName, int targetTeamId);
    Task SetTeamRegistrationStateAsync(bool isRegistrationOpen, DateTime? registrationCloseDate);
}
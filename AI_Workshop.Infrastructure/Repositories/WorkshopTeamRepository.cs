using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Interfaces;
using AI_Workshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AI_Workshop.Infrastructure.Repositories;

public class WorkshopTeamRepository : IWorkshopTeamRepository
{
    private readonly AIWorkshopDbContext _dbContext;

    public WorkshopTeamRepository(AIWorkshopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<WorkshopTeam>> GetAllTeamsAsync()
    {
        return await _dbContext.WorkshopTeams
            .AsNoTracking()
            .Include(team => team.Members)
            .OrderBy(team => team.TeamName)
            .ToArrayAsync();
    }

    public Task<WorkshopTeam?> GetTeamByIdAsync(int id)
    {
        return _dbContext.WorkshopTeams
            .AsNoTracking()
            .Include(team => team.Members)
            .FirstOrDefaultAsync(team => team.Id == id);
    }

    public Task<WorkshopTeam?> GetTeamByIdeaIdAsync(int ideaId)
    {
        return _dbContext.WorkshopTeams
            .AsNoTracking()
            .Include(team => team.Members)
            .FirstOrDefaultAsync(team => team.IdeaId == ideaId);
    }

    public async Task<WorkshopTeam> CreateTeamAsync(WorkshopTeam team)
    {
        _dbContext.WorkshopTeams.Add(team);
        await _dbContext.SaveChangesAsync();

        return await _dbContext.WorkshopTeams
            .AsNoTracking()
            .Include(item => item.Members)
            .FirstAsync(item => item.Id == team.Id);
    }

    public async Task<IReadOnlyCollection<WorkshopTeamMember>> GetTeamMembersAsync(int teamId)
    {
        return await _dbContext.WorkshopTeamMembers
            .AsNoTracking()
            .Where(member => member.TeamId == teamId)
            .OrderByDescending(member => member.IsTeamLeader)
            .ThenBy(member => member.EmployeeName)
            .ToArrayAsync();
    }

    public Task<WorkshopTeam?> GetEmployeeTeamAsync(string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        return _dbContext.WorkshopTeams
            .AsNoTracking()
            .Include(team => team.Members)
            .FirstOrDefaultAsync(team => team.Members.Any(member => member.EmployeeEmail != null && member.EmployeeEmail.ToLower() == normalizedEmail));
    }

    public async Task<WorkshopTeamMember?> EnsureTeamLeaderMemberAsync(int teamId, string employeeName, string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return null;
        }

        var team = await _dbContext.WorkshopTeams
            .Include(item => item.Members)
            .FirstOrDefaultAsync(item => item.Id == teamId);

        if (team is null)
        {
            return null;
        }

        var member = team.Members
            .FirstOrDefault(item => item.EmployeeEmail != null && item.EmployeeEmail.ToLower() == normalizedEmail);

        var hasChanges = false;

        if (member is null)
        {
            member = new WorkshopTeamMember
            {
                TeamId = teamId,
                EmployeeName = employeeName,
                EmployeeEmail = normalizedEmail,
                JoinedDate = DateTime.UtcNow,
                IsTeamLeader = true
            };

            _dbContext.WorkshopTeamMembers.Add(member);
            hasChanges = true;
        }
        else
        {
            if (!member.IsTeamLeader)
            {
                member.IsTeamLeader = true;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(employeeName) && !string.Equals(member.EmployeeName, employeeName, StringComparison.Ordinal))
            {
                member.EmployeeName = employeeName;
                hasChanges = true;
            }
        }

        foreach (var otherMember in team.Members.Where(item => item.Id != member.Id && item.IsTeamLeader))
        {
            otherMember.IsTeamLeader = false;
            hasChanges = true;
        }

        if (hasChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        return member;
    }

    public async Task<WorkshopTeamMember?> JoinTeamAsync(int teamId, string employeeName, string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        var team = await _dbContext.WorkshopTeams
            .Include(item => item.Members)
            .FirstOrDefaultAsync(item => item.Id == teamId);

        if (team is null)
        {
            return null;
        }

        var existing = team.Members.FirstOrDefault(member => NormalizeEmail(member.EmployeeEmail) == normalizedEmail);
        if (existing is not null)
        {
            return existing;
        }

        var created = new WorkshopTeamMember
        {
            TeamId = teamId,
            EmployeeName = employeeName,
            EmployeeEmail = normalizedEmail,
            JoinedDate = DateTime.UtcNow,
            IsTeamLeader = false
        };

        _dbContext.WorkshopTeamMembers.Add(created);
        await _dbContext.SaveChangesAsync();

        return created;
    }

    public async Task<bool> LeaveTeamAsync(int teamId, string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        var member = await _dbContext.WorkshopTeamMembers
            .FirstOrDefaultAsync(item => item.TeamId == teamId && item.EmployeeEmail != null && item.EmployeeEmail.ToLower() == normalizedEmail);

        if (member is null)
        {
            return false;
        }

        _dbContext.WorkshopTeamMembers.Remove(member);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SwitchTeamAsync(string employeeEmail, string employeeName, int targetTeamId)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var currentMember = await _dbContext.WorkshopTeamMembers
            .FirstOrDefaultAsync(member => member.EmployeeEmail != null && member.EmployeeEmail.ToLower() == normalizedEmail);

        if (currentMember is null)
        {
            return false;
        }

        if (currentMember.TeamId == targetTeamId)
        {
            return true;
        }

        var targetTeam = await _dbContext.WorkshopTeams
            .Include(team => team.Members)
            .FirstOrDefaultAsync(team => team.Id == targetTeamId);

        if (targetTeam is null)
        {
            return false;
        }

        if (targetTeam.Members.Any(member => member.EmployeeEmail != null && member.EmployeeEmail.ToLower() == normalizedEmail))
        {
            return true;
        }

        if (targetTeam.Members.Count >= targetTeam.MaxMembers)
        {
            return false;
        }

        _dbContext.WorkshopTeamMembers.Remove(currentMember);
        _dbContext.WorkshopTeamMembers.Add(new WorkshopTeamMember
        {
            TeamId = targetTeamId,
            EmployeeName = string.IsNullOrWhiteSpace(employeeName) ? currentMember.EmployeeName : employeeName,
            EmployeeEmail = normalizedEmail,
            JoinedDate = DateTime.UtcNow,
            IsTeamLeader = false
        });

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }

    public async Task SetTeamRegistrationStateAsync(bool isRegistrationOpen, DateTime? registrationCloseDate)
    {
        var teams = await _dbContext.WorkshopTeams.ToListAsync();
        foreach (var team in teams)
        {
            team.IsRegistrationOpen = isRegistrationOpen;
        }

        await _dbContext.SaveChangesAsync();
    }

    private static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToLowerInvariant();
    }
}

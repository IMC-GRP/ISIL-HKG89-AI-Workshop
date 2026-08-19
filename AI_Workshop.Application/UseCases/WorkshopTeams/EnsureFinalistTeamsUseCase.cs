using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Enums;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.WorkshopTeams;

public class EnsureFinalistTeamsUseCase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IWorkshopTeamRepository _workshopTeamRepository;

    public EnsureFinalistTeamsUseCase(IIdeaRepository ideaRepository, IWorkshopTeamRepository workshopTeamRepository)
    {
        _ideaRepository = ideaRepository;
        _workshopTeamRepository = workshopTeamRepository;
    }

    public async Task EnsureFinalistTeamsAsync()
    {
        var finalistIdeas = (await _ideaRepository.GetAllIdeasAsync())
            .Where(idea => idea.Status == IdeaStatus.Selected)
            .ToArray();

        var teamsByIdeaId = (await _workshopTeamRepository.GetAllTeamsAsync())
            .GroupBy(team => team.IdeaId)
            .ToDictionary(group => group.Key, group => group.First());

        foreach (var idea in finalistIdeas)
        {
            var team = await EnsureTeamExistsAsync(idea, teamsByIdeaId);
            await EnsureTeamLeaderMembershipAsync(idea, team);
        }
    }

    private async Task<WorkshopTeam> EnsureTeamExistsAsync(Idea idea, Dictionary<int, WorkshopTeam> teamsByIdeaId)
    {
        if (teamsByIdeaId.TryGetValue(idea.Id, out var existingTeam))
        {
            return existingTeam;
        }

        var createdTeam = await _workshopTeamRepository.CreateTeamAsync(new WorkshopTeam
        {
            IdeaId = idea.Id,
            TeamName = idea.Title?.Trim() ?? string.Empty,
            MaxMembers = WorkshopTeam.DefaultMaxMembers,
            CreatedDate = DateTime.UtcNow,
            IsRegistrationOpen = true
        });

        teamsByIdeaId[idea.Id] = createdTeam;
        return createdTeam;
    }

    private async Task EnsureTeamLeaderMembershipAsync(Idea idea, WorkshopTeam team)
    {
        var normalizedLeaderEmail = WorkshopTeamBusinessRules.NormalizeEmail(idea.TeamLeaderEmail);
        if (string.IsNullOrWhiteSpace(normalizedLeaderEmail))
        {
            return;
        }

        var existingTeamForLeader = await _workshopTeamRepository.GetEmployeeTeamAsync(normalizedLeaderEmail);
        if (existingTeamForLeader is not null && existingTeamForLeader.Id != team.Id)
        {
            await _workshopTeamRepository.LeaveTeamAsync(existingTeamForLeader.Id, normalizedLeaderEmail);
        }

        var leaderName = idea.TeamLeaderName?.Trim() ?? string.Empty;
        await _workshopTeamRepository.EnsureTeamLeaderMemberAsync(team.Id, leaderName, normalizedLeaderEmail);
    }
}

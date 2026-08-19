using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.WorkshopTeams;
using AI_Workshop.Domain.Enums;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetOrganizerDashboardSummary;

public class GetOrganizerDashboardSummaryUseCase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IWorkshopTeamRepository _workshopTeamRepository;
    private readonly IWorkshopSettingsRepository _workshopSettingsRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly EnsureFinalistTeamsUseCase _ensureFinalistTeamsUseCase;

    public GetOrganizerDashboardSummaryUseCase(
        IIdeaRepository ideaRepository,
        IWorkshopTeamRepository workshopTeamRepository,
        IWorkshopSettingsRepository workshopSettingsRepository,
        IScheduleRepository scheduleRepository,
        EnsureFinalistTeamsUseCase ensureFinalistTeamsUseCase)
    {
        _ideaRepository = ideaRepository;
        _workshopTeamRepository = workshopTeamRepository;
        _workshopSettingsRepository = workshopSettingsRepository;
        _scheduleRepository = scheduleRepository;
        _ensureFinalistTeamsUseCase = ensureFinalistTeamsUseCase;
    }

    public async Task<OrganizerDashboardSummaryDto> ExecuteAsync()
    {
        await _ensureFinalistTeamsUseCase.EnsureFinalistTeamsAsync();

        var ideas = await _ideaRepository.GetAllIdeasAsync();
        var allTeams = await _workshopTeamRepository.GetAllTeamsAsync();
        var schedule = await _scheduleRepository.GetCompleteScheduleAsync();
        var settings = await _workshopSettingsRepository.GetSettingsAsync();

        var finalists = ideas.Where(idea => idea.Status == IdeaStatus.Selected).ToArray();
        var finalistIdeaIds = finalists.Select(idea => idea.Id).ToHashSet();
        var teams = allTeams.Where(team => finalistIdeaIds.Contains(team.IdeaId)).ToArray();

        var registeredParticipants = teams.SelectMany(team => team.Members)
            .Count(member => !string.IsNullOrWhiteSpace(member.EmployeeEmail));
        var availableSpots = teams.Sum(team => Math.Max(0, team.MaxMembers - team.Members.Count));

        var membershipConflicts = teams
            .SelectMany(team => team.Members
                .Where(member => !string.IsNullOrWhiteSpace(member.EmployeeEmail))
                .Select(member => new { Email = WorkshopTeamBusinessRules.NormalizeEmail(member.EmployeeEmail), team.TeamName }))
            .GroupBy(item => item.Email)
            .Where(group => group.Select(g => g.TeamName).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1)
            .Select(group => new OrganizerMembershipConflictDto
            {
                EmployeeEmail = group.Key,
                TeamNames = group.Select(item => item.TeamName)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .ToArray()
            })
            .ToArray();

        return new OrganizerDashboardSummaryDto
        {
            TotalIdeas = ideas.Count,
            SubmittedIdeas = ideas.Count(idea => idea.Status == IdeaStatus.Submitted),
            FinalistIdeas = finalists.Length,
            RegisteredParticipants = registeredParticipants,
            AvailableTeamSpots = availableSpots,
            Registration = new WorkshopSettingsDto
            {
                IsTeamRegistrationOpen = settings.IsTeamRegistrationOpen,
                TeamRegistrationCloseDate = settings.TeamRegistrationCloseDate,
                IsTeamRegistrationCurrentlyOpen = WorkshopTeamBusinessRules.IsWorkshopRegistrationOpen(settings, DateTime.UtcNow)
            },
            IdeaStatusCounts =
            [
                new OrganizerIdeaStatusCountDto { Status = "Submitted", Count = ideas.Count(idea => idea.Status == IdeaStatus.Submitted) },
                new OrganizerIdeaStatusCountDto { Status = "Finalist", Count = finalists.Length },
                new OrganizerIdeaStatusCountDto { Status = "Not Selected", Count = ideas.Count(idea => idea.Status == IdeaStatus.NotSelected) }
            ],
            IdeaCategoryCounts = ideas
                .GroupBy(idea => idea.Category)
                .OrderBy(group => group.Key.ToString())
                .Select(group => new OrganizerIdeaCategoryCountDto
                {
                    Category = group.Key.ToString(),
                    Count = group.Count()
                })
                .ToArray(),
            MembershipConflicts = membershipConflicts,
            Readiness = new OrganizerReadinessDto
            {
                HasFourFinalists = finalists.Length == 4,
                HasFourTeams = teams.Length == 4,
                EachFinalistHasTeamLeader = teams.All(team => team.Members.Any(member => member.IsTeamLeader)),
                HasDayOneSchedule = schedule.Any(item => item.DayNumber == 1),
                HasDayTwoSchedule = schedule.Any(item => item.DayNumber == 2),
                IsRegistrationStillOpen = WorkshopTeamBusinessRules.IsWorkshopRegistrationOpen(settings, DateTime.UtcNow)
            }
        };
    }
}
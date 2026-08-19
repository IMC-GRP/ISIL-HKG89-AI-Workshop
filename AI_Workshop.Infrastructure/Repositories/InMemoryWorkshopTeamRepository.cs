using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Infrastructure.Repositories;

public class InMemoryWorkshopTeamRepository : IWorkshopTeamRepository
{
    private readonly object _syncLock = new();
    private readonly List<WorkshopTeam> _teams;
    private int _nextMemberId;

    public InMemoryWorkshopTeamRepository()
    {
        _teams =
        [
            new WorkshopTeam
            {
                Id = 1,
                IdeaId = 2,
                TeamName = "Anomaly Detection Finalist Team",
                MaxMembers = WorkshopTeam.DefaultMaxMembers,
                CreatedDate = DateTime.UtcNow.AddDays(-6),
                IsRegistrationOpen = true,
                RegistrationCloseDate = DateTime.UtcNow.AddDays(10),
                Members =
                [
                    new WorkshopTeamMember
                    {
                        Id = 1,
                        TeamId = 1,
                        EmployeeName = "Omar El-Sayed",
                        EmployeeEmail = "omar.elsayed@company.local",
                        JoinedDate = DateTime.UtcNow.AddDays(-2),
                        IsTeamLeader = true
                    },
                    new WorkshopTeamMember
                    {
                        Id = 2,
                        TeamId = 1,
                        EmployeeName = "Nadine Tarek",
                        EmployeeEmail = "nadine.tarek@company.local",
                        JoinedDate = DateTime.UtcNow.AddDays(-1),
                        IsTeamLeader = false
                    },
                    new WorkshopTeamMember
                    {
                        Id = 3,
                        TeamId = 1,
                        EmployeeName = "Karim Mostafa",
                        EmployeeEmail = "karim.mostafa@company.local",
                        JoinedDate = DateTime.UtcNow.AddHours(-18),
                        IsTeamLeader = false
                    }
                ]
            },
            new WorkshopTeam
            {
                Id = 2,
                IdeaId = 3,
                TeamName = "Release Readiness Finalist Team",
                MaxMembers = WorkshopTeam.DefaultMaxMembers,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                IsRegistrationOpen = true,
                RegistrationCloseDate = DateTime.UtcNow.AddDays(10),
                Members =
                [
                    new WorkshopTeamMember
                    {
                        Id = 4,
                        TeamId = 2,
                        EmployeeName = "Lina Farouk",
                        EmployeeEmail = "lina.farouk@company.local",
                        JoinedDate = DateTime.UtcNow.AddDays(-5),
                        IsTeamLeader = true
                    },
                    new WorkshopTeamMember
                    {
                        Id = 5,
                        TeamId = 2,
                        EmployeeName = "Mina Saleh",
                        EmployeeEmail = "mina.saleh@company.local",
                        JoinedDate = DateTime.UtcNow.AddDays(-4),
                        IsTeamLeader = false
                    },
                    new WorkshopTeamMember
                    {
                        Id = 6,
                        TeamId = 2,
                        EmployeeName = "Rana Hamed",
                        EmployeeEmail = "rana.hamed@company.local",
                        JoinedDate = DateTime.UtcNow.AddDays(-4),
                        IsTeamLeader = false
                    },
                    new WorkshopTeamMember
                    {
                        Id = 7,
                        TeamId = 2,
                        EmployeeName = "Tarek Naguib",
                        EmployeeEmail = "tarek.naguib@company.local",
                        JoinedDate = DateTime.UtcNow.AddDays(-3),
                        IsTeamLeader = false
                    },
                    new WorkshopTeamMember
                    {
                        Id = 8,
                        TeamId = 2,
                        EmployeeName = "Sara Emad",
                        EmployeeEmail = "sara.emad@company.local",
                        JoinedDate = DateTime.UtcNow.AddDays(-2),
                        IsTeamLeader = false
                    }
                ]
            },
            new WorkshopTeam
            {
                Id = 3,
                IdeaId = 4,
                TeamName = "Smart Support Finalist Team",
                MaxMembers = WorkshopTeam.DefaultMaxMembers,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                IsRegistrationOpen = true,
                RegistrationCloseDate = DateTime.UtcNow.AddDays(10),
                Members =
                [
                    new WorkshopTeamMember { Id = 9, TeamId = 3, EmployeeName = "Youssef Adel", EmployeeEmail = "youssef.adel@company.local", JoinedDate = DateTime.UtcNow.AddDays(-5), IsTeamLeader = true },
                    new WorkshopTeamMember { Id = 10, TeamId = 3, EmployeeName = "Noha Samir", EmployeeEmail = "noha.samir@company.local", JoinedDate = DateTime.UtcNow.AddDays(-4), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 11, TeamId = 3, EmployeeName = "Amr Salah", EmployeeEmail = "amr.salah@company.local", JoinedDate = DateTime.UtcNow.AddDays(-4), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 12, TeamId = 3, EmployeeName = "Mona Adel", EmployeeEmail = "mona.adel@company.local", JoinedDate = DateTime.UtcNow.AddDays(-3), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 13, TeamId = 3, EmployeeName = "Khaled Ismail", EmployeeEmail = "khaled.ismail@company.local", JoinedDate = DateTime.UtcNow.AddDays(-3), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 14, TeamId = 3, EmployeeName = "Nour Shaker", EmployeeEmail = "nour.shaker@company.local", JoinedDate = DateTime.UtcNow.AddDays(-2), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 15, TeamId = 3, EmployeeName = "Mahmoud Saad", EmployeeEmail = "mahmoud.saad@company.local", JoinedDate = DateTime.UtcNow.AddDays(-2), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 16, TeamId = 3, EmployeeName = "Dina Yassin", EmployeeEmail = "dina.yassin@company.local", JoinedDate = DateTime.UtcNow.AddDays(-1), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 17, TeamId = 3, EmployeeName = "Heba Mostafa", EmployeeEmail = "heba.mostafa@company.local", JoinedDate = DateTime.UtcNow.AddHours(-12), IsTeamLeader = false }
                ]
            },
            new WorkshopTeam
            {
                Id = 4,
                IdeaId = 5,
                TeamName = "Executive Insights Finalist Team",
                MaxMembers = WorkshopTeam.DefaultMaxMembers,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                IsRegistrationOpen = true,
                RegistrationCloseDate = DateTime.UtcNow.AddDays(10),
                Members =
                [
                    new WorkshopTeamMember { Id = 18, TeamId = 4, EmployeeName = "Mariam Nabil", EmployeeEmail = "mariam.nabil@company.local", JoinedDate = DateTime.UtcNow.AddDays(-5), IsTeamLeader = true },
                    new WorkshopTeamMember { Id = 19, TeamId = 4, EmployeeName = "Ahmed Hamdy", EmployeeEmail = "ahmed.hamdy@company.local", JoinedDate = DateTime.UtcNow.AddDays(-4), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 20, TeamId = 4, EmployeeName = "Salma Fathy", EmployeeEmail = "salma.fathy@company.local", JoinedDate = DateTime.UtcNow.AddDays(-4), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 21, TeamId = 4, EmployeeName = "Rami Ehab", EmployeeEmail = "rami.ehab@company.local", JoinedDate = DateTime.UtcNow.AddDays(-3), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 22, TeamId = 4, EmployeeName = "Yara Tamer", EmployeeEmail = "yara.tamer@company.local", JoinedDate = DateTime.UtcNow.AddDays(-3), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 23, TeamId = 4, EmployeeName = "Hany Ragab", EmployeeEmail = "hany.ragab@company.local", JoinedDate = DateTime.UtcNow.AddDays(-2), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 24, TeamId = 4, EmployeeName = "Laila Hesham", EmployeeEmail = "laila.hesham@company.local", JoinedDate = DateTime.UtcNow.AddDays(-2), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 25, TeamId = 4, EmployeeName = "Farah Ali", EmployeeEmail = "farah.ali@company.local", JoinedDate = DateTime.UtcNow.AddDays(-1), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 26, TeamId = 4, EmployeeName = "Ayman Nader", EmployeeEmail = "ayman.nader@company.local", JoinedDate = DateTime.UtcNow.AddDays(-1), IsTeamLeader = false },
                    new WorkshopTeamMember { Id = 27, TeamId = 4, EmployeeName = "Hoda Gerges", EmployeeEmail = "hoda.gerges@company.local", JoinedDate = DateTime.UtcNow.AddHours(-8), IsTeamLeader = false }
                ]
            }
        ];

        _nextMemberId = _teams.SelectMany(team => team.Members).Max(member => member.Id) + 1;
    }

    public Task<IReadOnlyCollection<WorkshopTeam>> GetAllTeamsAsync()
    {
        lock (_syncLock)
        {
            var snapshot = _teams
                .Select(CloneTeam)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<WorkshopTeam>>(snapshot);
        }
    }

    public Task<WorkshopTeam?> GetEmployeeTeamAsync(string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        lock (_syncLock)
        {
            var team = _teams.FirstOrDefault(t =>
                t.Members.Any(member => NormalizeEmail(member.EmployeeEmail) == normalizedEmail));

            return Task.FromResult(team is null ? null : CloneTeam(team));
        }
    }

    public Task<WorkshopTeamMember?> JoinTeamAsync(int teamId, string employeeName, string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        lock (_syncLock)
        {
            var team = _teams.FirstOrDefault(t => t.Id == teamId);
            if (team is null)
            {
                return Task.FromResult<WorkshopTeamMember?>(null);
            }

            var existing = team.Members.FirstOrDefault(member => NormalizeEmail(member.EmployeeEmail) == normalizedEmail);
            if (existing is not null)
            {
                return Task.FromResult<WorkshopTeamMember?>(CloneMember(existing));
            }

            var created = new WorkshopTeamMember
            {
                Id = _nextMemberId++,
                TeamId = team.Id,
                EmployeeName = employeeName,
                EmployeeEmail = normalizedEmail,
                JoinedDate = DateTime.UtcNow,
                IsTeamLeader = false
            };

            team.Members.Add(created);
            return Task.FromResult<WorkshopTeamMember?>(CloneMember(created));
        }
    }

    public Task<bool> LeaveTeamAsync(int teamId, string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        lock (_syncLock)
        {
            var team = _teams.FirstOrDefault(t => t.Id == teamId);
            if (team is null)
            {
                return Task.FromResult(false);
            }

            var member = team.Members.FirstOrDefault(item => NormalizeEmail(item.EmployeeEmail) == normalizedEmail);
            if (member is null)
            {
                return Task.FromResult(false);
            }

            team.Members.Remove(member);
            return Task.FromResult(true);
        }
    }

    public Task<bool> SwitchTeamAsync(string employeeEmail, string employeeName, int targetTeamId)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);

        lock (_syncLock)
        {
            var currentTeam = _teams.FirstOrDefault(t =>
                t.Members.Any(member => NormalizeEmail(member.EmployeeEmail) == normalizedEmail));
            var targetTeam = _teams.FirstOrDefault(t => t.Id == targetTeamId);

            if (currentTeam is null || targetTeam is null)
            {
                return Task.FromResult(false);
            }

            if (currentTeam.Id == targetTeam.Id)
            {
                return Task.FromResult(true);
            }

            var currentMember = currentTeam.Members.First(member => NormalizeEmail(member.EmployeeEmail) == normalizedEmail);
            if (targetTeam.Members.Any(member => NormalizeEmail(member.EmployeeEmail) == normalizedEmail))
            {
                return Task.FromResult(true);
            }

            if (targetTeam.Members.Count >= targetTeam.MaxMembers)
            {
                return Task.FromResult(false);
            }

            currentTeam.Members.Remove(currentMember);
            targetTeam.Members.Add(new WorkshopTeamMember
            {
                Id = _nextMemberId++,
                TeamId = targetTeam.Id,
                EmployeeName = string.IsNullOrWhiteSpace(employeeName) ? currentMember.EmployeeName : employeeName,
                EmployeeEmail = normalizedEmail,
                JoinedDate = DateTime.UtcNow,
                IsTeamLeader = false
            });

            return Task.FromResult(true);
        }
    }

    public Task SetTeamRegistrationStateAsync(bool isRegistrationOpen, DateTime? registrationCloseDate)
    {
        lock (_syncLock)
        {
            foreach (var team in _teams)
            {
                team.IsRegistrationOpen = isRegistrationOpen;
                team.RegistrationCloseDate = registrationCloseDate;
            }

            return Task.CompletedTask;
        }
    }

    public Task<WorkshopTeam?> GetTeamByIdAsync(int id)
    {
        lock (_syncLock)
        {
            var team = _teams.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(team is null ? null : CloneTeam(team));
        }
    }

    public Task<WorkshopTeam?> GetTeamByIdeaIdAsync(int ideaId)
    {
        lock (_syncLock)
        {
            var team = _teams.FirstOrDefault(t => t.IdeaId == ideaId);
            return Task.FromResult(team is null ? null : CloneTeam(team));
        }
    }

    public Task<WorkshopTeam> CreateTeamAsync(WorkshopTeam team)
    {
        lock (_syncLock)
        {
            var newId = _teams.Count == 0 ? 1 : _teams.Max(item => item.Id) + 1;
            var created = new WorkshopTeam
            {
                Id = newId,
                IdeaId = team.IdeaId,
                TeamName = team.TeamName,
                MaxMembers = team.MaxMembers,
                CreatedDate = team.CreatedDate,
                IsRegistrationOpen = team.IsRegistrationOpen,
                RegistrationCloseDate = team.RegistrationCloseDate,
                Members = []
            };

            _teams.Add(created);
            return Task.FromResult(CloneTeam(created));
        }
    }

    public Task<WorkshopTeamMember?> EnsureTeamLeaderMemberAsync(int teamId, string employeeName, string employeeEmail)
    {
        var normalizedEmail = NormalizeEmail(employeeEmail);
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return Task.FromResult<WorkshopTeamMember?>(null);
        }

        lock (_syncLock)
        {
            var team = _teams.FirstOrDefault(t => t.Id == teamId);
            if (team is null)
            {
                return Task.FromResult<WorkshopTeamMember?>(null);
            }

            var existing = team.Members.FirstOrDefault(member => NormalizeEmail(member.EmployeeEmail) == normalizedEmail);
            if (existing is null)
            {
                var created = new WorkshopTeamMember
                {
                    Id = _nextMemberId++,
                    TeamId = teamId,
                    EmployeeName = employeeName,
                    EmployeeEmail = normalizedEmail,
                    JoinedDate = DateTime.UtcNow,
                    IsTeamLeader = true
                };

                team.Members.Add(created);

                foreach (var otherMember in team.Members.Where(member => member.Id != created.Id && member.IsTeamLeader))
                {
                    otherMember.IsTeamLeader = false;
                }

                return Task.FromResult<WorkshopTeamMember?>(CloneMember(created));
            }

            existing.IsTeamLeader = true;
            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                existing.EmployeeName = employeeName;
            }

            foreach (var otherMember in team.Members.Where(member => member.Id != existing.Id && member.IsTeamLeader))
            {
                otherMember.IsTeamLeader = false;
            }

            return Task.FromResult<WorkshopTeamMember?>(CloneMember(existing));
        }
    }

    public Task<IReadOnlyCollection<WorkshopTeamMember>> GetTeamMembersAsync(int teamId)
    {
        lock (_syncLock)
        {
            var members = _teams
                .Where(team => team.Id == teamId)
                .SelectMany(team => team.Members)
                .Select(CloneMember)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<WorkshopTeamMember>>(members);
        }
    }

    private static WorkshopTeam CloneTeam(WorkshopTeam team)
    {
        return new WorkshopTeam
        {
            Id = team.Id,
            IdeaId = team.IdeaId,
            TeamName = team.TeamName,
            MaxMembers = team.MaxMembers,
            CreatedDate = team.CreatedDate,
            IsRegistrationOpen = team.IsRegistrationOpen,
            RegistrationCloseDate = team.RegistrationCloseDate,
            Members = team.Members.Select(CloneMember).ToList()
        };
    }

    private static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static WorkshopTeamMember CloneMember(WorkshopTeamMember member)
    {
        return new WorkshopTeamMember
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
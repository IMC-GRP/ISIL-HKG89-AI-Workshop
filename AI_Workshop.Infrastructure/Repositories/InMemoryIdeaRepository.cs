using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Enums;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Infrastructure.Repositories;

public class InMemoryIdeaRepository : IIdeaRepository
{
    private readonly object _syncLock = new();
    private readonly List<Idea> _ideas;
    private int _nextId;

    public InMemoryIdeaRepository()
    {
        _ideas = new List<Idea>
        {
            new()
            {
                Id = 1,
                Title = "AI Documentation Assistant",
                Category = IdeaCategory.AI,
                Description = "Create a contextual assistant that answers engineering documentation questions instantly.",
                TeamLeaderName = "Nora Ibrahim",
                TeamLeaderEmail = "nora.ibrahim@company.local",
                ProblemToSolve = "Teams spend too much time searching fragmented project documentation and onboarding notes.",
                ProposedSolution = "Index internal docs and use retrieval-augmented AI responses with source references.",
                TargetUsers = "Engineering and product teams",
                ToolsAndTechnologies = "Azure OpenAI, Semantic Kernel, ASP.NET Core, Vector Search",
                ExpectedValue = "Faster onboarding and fewer repeated technical support questions.",
                AdditionalNotes = "Start with architecture guides and runbooks.",
                SubmittedBy = "Nora Ibrahim",
                SubmittedDate = DateTime.UtcNow.AddDays(-5),
                Status = IdeaStatus.Submitted
            },
            new()
            {
                Id = 2,
                Title = "Production Anomaly Detection Hub",
                Category = IdeaCategory.DataAndAnalytics,
                Description = "Detect operational anomalies across logs and telemetry before incidents escalate.",
                TeamLeaderName = "Omar El-Sayed",
                TeamLeaderEmail = "omar.elsayed@company.local",
                ProblemToSolve = "Critical production anomalies are often discovered late across disconnected monitoring tools.",
                ProposedSolution = "Combine metrics, logs, and deployment events with anomaly scoring and root-cause hints.",
                TargetUsers = "SRE and platform operations",
                ToolsAndTechnologies = "Azure Monitor, Kusto, ML.NET, Power BI",
                ExpectedValue = "Reduced incident duration and earlier detection of risky behavior.",
                AdditionalNotes = "This idea aligns with quarterly reliability goals.",
                SubmittedBy = "Omar El-Sayed",
                SubmittedDate = DateTime.UtcNow.AddDays(-4),
                Status = IdeaStatus.Selected
            },
            new()
            {
                Id = 3,
                Title = "Automated Release Readiness Checker",
                Category = IdeaCategory.Automation,
                Description = "Automate release gating checks for quality, security, and operational readiness.",
                TeamLeaderName = "Lina Farouk",
                TeamLeaderEmail = "lina.farouk@company.local",
                ProblemToSolve = "Release approvals rely on manual checklists and can miss critical readiness steps.",
                ProposedSolution = "Automate checklist evaluation from pipeline artifacts and quality scan outputs.",
                TargetUsers = "Engineering managers and release coordinators",
                ToolsAndTechnologies = "GitHub Actions, .NET, Copilot, SonarQube",
                ExpectedValue = "More predictable releases and lower rollout risk.",
                AdditionalNotes = "Should integrate with existing CI workflows.",
                SubmittedBy = "Lina Farouk",
                SubmittedDate = DateTime.UtcNow.AddDays(-3),
                Status = IdeaStatus.Selected
            },
            new()
            {
                Id = 4,
                Title = "Smart Support Assistant",
                Category = IdeaCategory.AI,
                Description = "Suggest response drafts and knowledge articles for internal support requests.",
                TeamLeaderName = "Youssef Adel",
                TeamLeaderEmail = "youssef.adel@company.local",
                ProblemToSolve = "Support specialists spend significant time writing repetitive ticket responses.",
                ProposedSolution = "Use AI-assisted response generation tied to approved support knowledge.",
                TargetUsers = "IT helpdesk and HR operations support",
                ToolsAndTechnologies = "Azure OpenAI, Blazor, ASP.NET Core API",
                ExpectedValue = "Improved support throughput and more consistent responses.",
                AdditionalNotes = null,
                SubmittedBy = "Youssef Adel",
                SubmittedDate = DateTime.UtcNow.AddDays(-2),
                Status = IdeaStatus.Selected
            },
            new()
            {
                Id = 5,
                Title = "Executive Insight Dashboard",
                Category = IdeaCategory.DataAndAnalytics,
                Description = "Build an AI-assisted dashboard that highlights business risks and opportunities.",
                TeamLeaderName = "Mariam Nabil",
                TeamLeaderEmail = "mariam.nabil@company.local",
                ProblemToSolve = "Leadership lacks a single dynamic view of strategic KPIs and trend alerts.",
                ProposedSolution = "Aggregate key metrics with trend narratives and predictive indicators.",
                TargetUsers = "Department heads and executive leadership",
                ToolsAndTechnologies = "Power BI, Fabric, Python, OpenAI",
                ExpectedValue = "Faster decision cycles and improved strategic visibility.",
                AdditionalNotes = "Use role-based visibility for sensitive metrics.",
                SubmittedBy = "Mariam Nabil",
                SubmittedDate = DateTime.UtcNow.AddDays(-1),
                Status = IdeaStatus.Selected
            },
            new()
            {
                Id = 6,
                Title = "Automated Quality Inspection for Claims",
                Category = IdeaCategory.ProcessImprovement,
                Description = "Automatically review claim records for missing data and policy compliance gaps.",
                TeamLeaderName = "Hassan Rami",
                TeamLeaderEmail = "hassan.rami@company.local",
                ProblemToSolve = "Manual quality audits are slow and cover only a small sample of claims.",
                ProposedSolution = "Create rule and AI-based checks to flag incomplete or inconsistent claims.",
                TargetUsers = "Operations quality assurance teams",
                ToolsAndTechnologies = "C#, Azure Functions, Cognitive Services, SQL",
                ExpectedValue = "Higher quality consistency and reduced rework.",
                AdditionalNotes = "Pilot with one business unit first.",
                SubmittedBy = "Hassan Rami",
                SubmittedDate = DateTime.UtcNow.AddHours(-6),
                Status = IdeaStatus.NotSelected
            }
        };

        _nextId = _ideas.Max(idea => idea.Id) + 1;
    }

    public Task<IReadOnlyCollection<Idea>> GetAllIdeasAsync()
    {
        lock (_syncLock)
        {
            var snapshot = _ideas
                .OrderByDescending(idea => idea.SubmittedDate)
                .Select(CloneIdea)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Idea>>(snapshot);
        }
    }

    public Task<Idea?> GetIdeaByIdAsync(int id)
    {
        lock (_syncLock)
        {
            var idea = _ideas.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(idea is null ? null : CloneIdea(idea));
        }
    }

    public Task<Idea> AddIdeaAsync(Idea idea)
    {
        lock (_syncLock)
        {
            var itemToStore = CloneIdea(idea);
            itemToStore.Id = _nextId++;

            _ideas.Add(itemToStore);

            return Task.FromResult(CloneIdea(itemToStore));
        }
    }

    private static Idea CloneIdea(Idea idea)
    {
        return new Idea
        {
            Id = idea.Id,
            Title = idea.Title,
            Category = idea.Category,
            Description = idea.Description,
            TeamLeaderName = idea.TeamLeaderName,
            TeamLeaderEmail = idea.TeamLeaderEmail,
            ProblemToSolve = idea.ProblemToSolve,
            ProposedSolution = idea.ProposedSolution,
            TargetUsers = idea.TargetUsers,
            ToolsAndTechnologies = idea.ToolsAndTechnologies,
            ExpectedValue = idea.ExpectedValue,
            AdditionalNotes = idea.AdditionalNotes,
            SubmittedBy = idea.SubmittedBy,
            SubmittedDate = idea.SubmittedDate,
            Status = idea.Status
        };
    }
}

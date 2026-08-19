using AI_Workshop.Domain.Enums;

namespace AI_Workshop.Application.DTOs;

public class IdeaDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public IdeaCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string TeamLeaderName { get; set; } = string.Empty;
    public string? TeamLeaderEmail { get; set; }
    public string ProblemToSolve { get; set; } = string.Empty;
    public string ProposedSolution { get; set; } = string.Empty;
    public string TargetUsers { get; set; } = string.Empty;
    public string ToolsAndTechnologies { get; set; } = string.Empty;
    public string ExpectedValue { get; set; } = string.Empty;
    public string? AdditionalNotes { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public IdeaStatus Status { get; set; }
}

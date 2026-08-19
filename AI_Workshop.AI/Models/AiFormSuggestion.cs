namespace AI_Workshop.AI.Models;

public sealed class AiFormSuggestion
{
    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? ProblemToSolve { get; set; }
    public string? ProposedSolution { get; set; }
    public string? TargetUsers { get; set; }
    public string? ToolsAndTechnologies { get; set; }
    public string? ExpectedValue { get; set; }
    public string? AdditionalNotes { get; set; }
    public string? TeamLeaderName { get; set; }
    public string? TeamLeaderEmail { get; set; }
}

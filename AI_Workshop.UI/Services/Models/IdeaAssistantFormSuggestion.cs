namespace AI_Workshop.UI.Services.Models;

public sealed class IdeaAssistantFormSuggestion
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
}

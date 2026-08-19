using System.ComponentModel.DataAnnotations;
using AI_Workshop.Domain.Enums;

namespace AI_Workshop.API.Contracts;

public class CreateIdeaRequest
{
    [Required(AllowEmptyStrings = false)]
    public string? Title { get; set; }

    [Required]
    public IdeaCategory? Category { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? Description { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? TeamLeaderName { get; set; }

    [EmailAddress]
    public string? TeamLeaderEmail { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? ProblemToSolve { get; set; }

    public string? ProposedSolution { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? TargetUsers { get; set; }

    public string? ToolsAndTechnologies { get; set; }

    public string? ExpectedValue { get; set; }

    public string? AdditionalNotes { get; set; }

    public string? SubmittedBy { get; set; }
}

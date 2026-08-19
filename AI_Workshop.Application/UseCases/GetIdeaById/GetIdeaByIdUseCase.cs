using AI_Workshop.Application.DTOs;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetIdeaById;

public class GetIdeaByIdUseCase
{
    private readonly IIdeaRepository _ideaRepository;

    public GetIdeaByIdUseCase(IIdeaRepository ideaRepository)
    {
        _ideaRepository = ideaRepository;
    }

    public async Task<IdeaDto?> ExecuteAsync(int id)
    {
        var idea = await _ideaRepository.GetIdeaByIdAsync(id);
        if (idea is null)
        {
            return null;
        }

        return new IdeaDto
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

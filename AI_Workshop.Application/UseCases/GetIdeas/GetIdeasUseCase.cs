using AI_Workshop.Application.DTOs;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.GetIdeas;

public class GetIdeasUseCase
{
    private readonly IIdeaRepository _ideaRepository;

    public GetIdeasUseCase(IIdeaRepository ideaRepository)
    {
        _ideaRepository = ideaRepository;
    }

    public async Task<IReadOnlyCollection<IdeaDto>> ExecuteAsync()
    {
        var ideas = await _ideaRepository.GetAllIdeasAsync();

        return ideas
            .Select(idea => new IdeaDto
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
            })
            .ToArray();
    }
}

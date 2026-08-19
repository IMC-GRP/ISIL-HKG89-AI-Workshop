using AI_Workshop.Application.DTOs;
using AI_Workshop.Domain.Entities;
using AI_Workshop.Domain.Enums;
using AI_Workshop.Domain.Interfaces;

namespace AI_Workshop.Application.UseCases.CreateIdea;

public class CreateIdeaUseCase
{
    private readonly IIdeaRepository _ideaRepository;

    public CreateIdeaUseCase(IIdeaRepository ideaRepository)
    {
        _ideaRepository = ideaRepository;
    }

    public async Task<IdeaDto> ExecuteAsync(CreateIdeaDto dto)
    {
        var idea = new Idea
        {
            Title = dto.Title,
            Category = dto.Category,
            Description = dto.Description,
            TeamLeaderName = dto.TeamLeaderName,
            TeamLeaderEmail = dto.TeamLeaderEmail,
            ProblemToSolve = dto.ProblemToSolve,
            ProposedSolution = dto.ProposedSolution,
            TargetUsers = dto.TargetUsers,
            ToolsAndTechnologies = dto.ToolsAndTechnologies,
            ExpectedValue = dto.ExpectedValue,
            AdditionalNotes = dto.AdditionalNotes,
            SubmittedBy = dto.SubmittedBy,
            SubmittedDate = DateTime.UtcNow,
            Status = IdeaStatus.Submitted
        };

        var createdIdea = await _ideaRepository.AddIdeaAsync(idea);

        return new IdeaDto
        {
            Id = createdIdea.Id,
            Title = createdIdea.Title,
            Category = createdIdea.Category,
            Description = createdIdea.Description,
            TeamLeaderName = createdIdea.TeamLeaderName,
            TeamLeaderEmail = createdIdea.TeamLeaderEmail,
            ProblemToSolve = createdIdea.ProblemToSolve,
            ProposedSolution = createdIdea.ProposedSolution,
            TargetUsers = createdIdea.TargetUsers,
            ToolsAndTechnologies = createdIdea.ToolsAndTechnologies,
            ExpectedValue = createdIdea.ExpectedValue,
            AdditionalNotes = createdIdea.AdditionalNotes,
            SubmittedBy = createdIdea.SubmittedBy,
            SubmittedDate = createdIdea.SubmittedDate,
            Status = createdIdea.Status
        };
    }
}

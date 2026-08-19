using AI_Workshop.API.Contracts;
using AI_Workshop.AI.Exceptions;
using AI_Workshop.AI.Interfaces;
using AI_Workshop.AI.Models;
using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.CreateIdea;
using AI_Workshop.Application.UseCases.GetIdeaById;
using AI_Workshop.Application.UseCases.GetIdeas;
using Microsoft.AspNetCore.Mvc;

namespace AI_Workshop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdeasController : ControllerBase
{
    private readonly GetIdeasUseCase _getIdeasUseCase;
    private readonly GetIdeaByIdUseCase _getIdeaByIdUseCase;
    private readonly CreateIdeaUseCase _createIdeaUseCase;
    private readonly IIdeaAssistantService _ideaAssistantService;

    public IdeasController(
        GetIdeasUseCase getIdeasUseCase,
        GetIdeaByIdUseCase getIdeaByIdUseCase,
        CreateIdeaUseCase createIdeaUseCase,
        IIdeaAssistantService ideaAssistantService)
    {
        _getIdeasUseCase = getIdeasUseCase;
        _getIdeaByIdUseCase = getIdeaByIdUseCase;
        _createIdeaUseCase = createIdeaUseCase;
        _ideaAssistantService = ideaAssistantService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<IdeaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<IdeaDto>>> GetIdeas()
    {
        var ideas = await _getIdeasUseCase.ExecuteAsync();
        return Ok(ideas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(IdeaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IdeaDto>> GetIdeaById(int id)
    {
        var idea = await _getIdeaByIdUseCase.ExecuteAsync(id);
        if (idea is null)
        {
            return NotFound();
        }

        return Ok(idea);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IdeaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IdeaDto>> CreateIdea([FromBody] CreateIdeaRequest request)
    {
        var createDto = new CreateIdeaDto
        {
            Title = request.Title?.Trim() ?? string.Empty,
            Category = request.Category ?? default,
            Description = request.Description?.Trim() ?? string.Empty,
            TeamLeaderName = request.TeamLeaderName?.Trim() ?? string.Empty,
            TeamLeaderEmail = string.IsNullOrWhiteSpace(request.TeamLeaderEmail) ? null : request.TeamLeaderEmail.Trim(),
            ProblemToSolve = request.ProblemToSolve?.Trim() ?? string.Empty,
            ProposedSolution = request.ProposedSolution?.Trim() ?? string.Empty,
            TargetUsers = request.TargetUsers?.Trim() ?? string.Empty,
            ToolsAndTechnologies = request.ToolsAndTechnologies?.Trim() ?? string.Empty,
            ExpectedValue = request.ExpectedValue?.Trim() ?? string.Empty,
            AdditionalNotes = string.IsNullOrWhiteSpace(request.AdditionalNotes) ? null : request.AdditionalNotes.Trim(),
            SubmittedBy = string.IsNullOrWhiteSpace(request.SubmittedBy) ? request.TeamLeaderName?.Trim() ?? string.Empty : request.SubmittedBy.Trim()
        };

        var createdIdea = await _createIdeaUseCase.ExecuteAsync(createDto);

        return CreatedAtAction(nameof(GetIdeaById), new { id = createdIdea.Id }, createdIdea);
    }

    [HttpPost("assistant/chat")]
    [HttpPost("/api/idea-assistant/chat")]
    [ProducesResponseType(typeof(IdeaAssistantChatResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IdeaAssistantChatResponse>> ChatWithAssistant([FromBody] IdeaAssistantChatRequest request, CancellationToken cancellationToken)
    {
        var assistantRequest = new AiIdeaAssistantChatRequest
        {
            Message = request.Message?.Trim() ?? string.Empty,
            ConversationId = request.ConversationId,
            CurrentForm = request.CurrentForm is null
                ? null
                : new AiFormSuggestion
                {
                    Title = request.CurrentForm.Title,
                    Category = request.CurrentForm.Category,
                    Description = request.CurrentForm.Description,
                    ProblemToSolve = request.CurrentForm.ProblemToSolve,
                    ProposedSolution = request.CurrentForm.ProposedSolution,
                    TargetUsers = request.CurrentForm.TargetUsers,
                    ToolsAndTechnologies = request.CurrentForm.ToolsAndTechnologies,
                    ExpectedValue = request.CurrentForm.ExpectedValue,
                    AdditionalNotes = request.CurrentForm.AdditionalNotes
                },
            Conversation = request.Conversation
                .Select(message => new AiConversationMessage
                {
                    Role = message.Role,
                    Content = message.Content
                })
                .ToArray()
        };

        try
        {
            var assistantResponse = await _ideaAssistantService.ChatAsync(assistantRequest, cancellationToken);

            return Ok(new IdeaAssistantChatResponse
            {
                AssistantMessage = assistantResponse.AssistantMessage,
                ConversationId = assistantResponse.ConversationId,
                UpdatedFields = assistantResponse.UpdatedFields,
                FormUpdates = assistantResponse.FormUpdates is null
                    ? null
                    : new IdeaAssistantFormSuggestion
                    {
                        Title = assistantResponse.FormUpdates.Title,
                        Category = assistantResponse.FormUpdates.Category,
                        Description = assistantResponse.FormUpdates.Description,
                        ProblemToSolve = assistantResponse.FormUpdates.ProblemToSolve,
                        ProposedSolution = assistantResponse.FormUpdates.ProposedSolution,
                        TargetUsers = assistantResponse.FormUpdates.TargetUsers,
                        ToolsAndTechnologies = assistantResponse.FormUpdates.ToolsAndTechnologies,
                        ExpectedValue = assistantResponse.FormUpdates.ExpectedValue,
                        AdditionalNotes = assistantResponse.FormUpdates.AdditionalNotes
                    }
            });
        }
        catch (AiAssistantUnavailableException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = ex.Message });
        }
    }
}

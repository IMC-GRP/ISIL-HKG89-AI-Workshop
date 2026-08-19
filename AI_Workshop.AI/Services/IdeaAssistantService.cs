using AI_Workshop.AI.Configuration;
using AI_Workshop.AI.Exceptions;
using AI_Workshop.AI.Interfaces;
using AI_Workshop.AI.Models;
using AI_Workshop.AI.Prompts;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AI_Workshop.AI.Services;

public sealed class IdeaAssistantService : IIdeaAssistantService
{
    private readonly AiAgentOptions _options;
    private readonly ChatClientAgent _agent;
    private readonly ILogger<IdeaAssistantService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private const string ResponseContractInstructions =
        "Return only valid JSON with this exact shape: " +
        "{\"assistantMessage\":\"string\",\"formSuggestion\":{\"title\":string|null,\"category\":string|null,\"description\":string|null,\"problemToSolve\":string|null,\"proposedSolution\":string|null,\"targetUsers\":string|null,\"toolsAndTechnologies\":string|null,\"expectedValue\":string|null,\"additionalNotes\":string|null}}. " +
        "Do not add markdown, code fences, or extra fields. If a field is unknown return null. formSuggestion values must be in English.";

    public IdeaAssistantService(
        IOptions<AiAgentOptions> options,
        ChatClientAgent agent,
        ILogger<IdeaAssistantService> logger)
    {
        _options = options.Value;
        _agent = agent;
        _logger = logger;
    }

    public async Task<AiIdeaAssistantChatResponse> ChatAsync(AiIdeaAssistantChatRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ArgumentException("Message is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(_options.BaseUrl) ||
            string.IsNullOrWhiteSpace(_options.Model) ||
            string.IsNullOrWhiteSpace(_options.SubscriptionKey) ||
            string.IsNullOrWhiteSpace(_options.AppId))
        {
            throw new AiAssistantUnavailableException("AI assistant is not configured. Set AiAgent:BaseUrl, AiAgent:Model, AiAgent:AppId, and AiAgent:SubscriptionKey.");
        }

        try
        {
            var messages = BuildMessages(request);
            var runOptions = new ChatClientAgentRunOptions(new ChatOptions
            {
                ModelId = _options.Model
            });

            var result = await _agent.RunAsync<StructuredAssistantResponse>(
                messages,
                session: null,
                JsonOptions,
                runOptions,
                cancellationToken);

            var payload = result.Result;
            if (payload is null)
            {
                throw new AiAssistantUnavailableException("I couldn't process the assistant response. Please try again.");
            }

            var formUpdates = MapSuggestion(payload.FormSuggestion);

            return new AiIdeaAssistantChatResponse
            {
                AssistantMessage = string.IsNullOrWhiteSpace(payload.AssistantMessage)
                    ? "I can help refine your idea. What impact do you expect from this solution?"
                    : payload.AssistantMessage.Trim(),
                ConversationId = request.ConversationId,
                FormUpdates = formUpdates,
                UpdatedFields = GetUpdatedFields(formUpdates)
            };
        }
        catch (AiAssistantUnavailableException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI assistant request failed. Model: {Model}", _options.Model);
            throw new AiAssistantUnavailableException(
                "I'm having trouble connecting right now. You can continue filling the form manually.");
        }
    }

    private List<ChatMessage> BuildMessages(AiIdeaAssistantChatRequest request)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, $"{IdeaAssistantPrompt.SystemInstructions} {ResponseContractInstructions}")
        };

        var formContext = BuildFormContext(request.CurrentForm);
        if (!string.IsNullOrWhiteSpace(formContext))
        {
            messages.Add(new ChatMessage(ChatRole.System, formContext));
        }

        foreach (var message in request.Conversation)
        {
            var role = ParseRole(message.Role);
            if (role is null || string.IsNullOrWhiteSpace(message.Content))
            {
                continue;
            }

            messages.Add(new ChatMessage(role.Value, message.Content.Trim()));
        }

        if (!request.Conversation.Any(m => string.Equals(m.Role, "user", StringComparison.OrdinalIgnoreCase) && string.Equals(m.Content?.Trim(), request.Message.Trim(), StringComparison.Ordinal)))
        {
            messages.Add(new ChatMessage(ChatRole.User, request.Message.Trim()));
        }

        return messages;
    }

    private static ChatRole? ParseRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return null;
        }

        return role.Trim().ToLowerInvariant() switch
        {
            "assistant" => ChatRole.Assistant,
            "user" => ChatRole.User,
            "system" => ChatRole.System,
            _ => null
        };
    }

    private static string BuildFormContext(AiFormSuggestion? form)
    {
        if (form is null)
        {
            return string.Empty;
        }

        return
            "Current form context (preserve and improve where relevant): " +
            $"Title='{form.Title}', Category='{form.Category}', Description='{form.Description}', " +
            $"ProblemToSolve='{form.ProblemToSolve}', ProposedSolution='{form.ProposedSolution}', " +
            $"TargetUsers='{form.TargetUsers}', ToolsAndTechnologies='{form.ToolsAndTechnologies}', " +
            $"ExpectedValue='{form.ExpectedValue}', AdditionalNotes='{form.AdditionalNotes}'.";
    }

    private static AiFormSuggestion? MapSuggestion(StructuredFormSuggestion? suggestion)
    {
        if (suggestion is null)
        {
            return null;
        }

        return new AiFormSuggestion
        {
            Title = Normalize(suggestion.Title),
            Category = NormalizeCategory(suggestion.Category),
            Description = Normalize(suggestion.Description),
            ProblemToSolve = Normalize(suggestion.ProblemToSolve),
            ProposedSolution = Normalize(suggestion.ProposedSolution),
            TargetUsers = Normalize(suggestion.TargetUsers),
            ToolsAndTechnologies = Normalize(suggestion.ToolsAndTechnologies),
            ExpectedValue = Normalize(suggestion.ExpectedValue),
            AdditionalNotes = Normalize(suggestion.AdditionalNotes)
        };
    }

    private static IReadOnlyCollection<string> GetUpdatedFields(AiFormSuggestion? suggestion)
    {
        if (suggestion is null)
        {
            return [];
        }

        var updated = new List<string>();
        AddIfSet(updated, nameof(AiFormSuggestion.Title), suggestion.Title);
        AddIfSet(updated, nameof(AiFormSuggestion.Category), suggestion.Category);
        AddIfSet(updated, nameof(AiFormSuggestion.Description), suggestion.Description);
        AddIfSet(updated, nameof(AiFormSuggestion.ProblemToSolve), suggestion.ProblemToSolve);
        AddIfSet(updated, nameof(AiFormSuggestion.ProposedSolution), suggestion.ProposedSolution);
        AddIfSet(updated, nameof(AiFormSuggestion.TargetUsers), suggestion.TargetUsers);
        AddIfSet(updated, nameof(AiFormSuggestion.ToolsAndTechnologies), suggestion.ToolsAndTechnologies);
        AddIfSet(updated, nameof(AiFormSuggestion.ExpectedValue), suggestion.ExpectedValue);
        AddIfSet(updated, nameof(AiFormSuggestion.AdditionalNotes), suggestion.AdditionalNotes);
        return updated;
    }

    private static void AddIfSet(ICollection<string> updated, string fieldName, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            updated.Add(fieldName);
        }
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeCategory(string? category)
    {
        var normalized = Normalize(category);
        if (normalized is null)
        {
            return null;
        }

        return normalized.Replace(" ", string.Empty).Replace("&", "And", StringComparison.OrdinalIgnoreCase);
    }

    private sealed class StructuredAssistantResponse
    {
        public string AssistantMessage { get; set; } = string.Empty;

        public StructuredFormSuggestion? FormSuggestion { get; set; }
    }

    private sealed class StructuredFormSuggestion
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
}

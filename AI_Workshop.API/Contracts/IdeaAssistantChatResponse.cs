namespace AI_Workshop.API.Contracts;

public sealed class IdeaAssistantChatResponse
{
    public string AssistantMessage { get; set; } = string.Empty;

    public string? ConversationId { get; set; }

    public IdeaAssistantFormSuggestion? FormUpdates { get; set; }

    public IReadOnlyCollection<string> UpdatedFields { get; set; } = [];
}

namespace AI_Workshop.AI.Models;

public sealed class AiIdeaAssistantChatResponse
{
    public string AssistantMessage { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
    public AiFormSuggestion? FormUpdates { get; set; }
    public IReadOnlyCollection<string> UpdatedFields { get; set; } = [];

    public bool HasFormUpdates => FormUpdates is not null;
}

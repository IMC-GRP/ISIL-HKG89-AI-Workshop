namespace AI_Workshop.AI.Models;

public sealed class AiIdeaAssistantChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
    public AiFormSuggestion? CurrentForm { get; set; }
    public IReadOnlyCollection<AiConversationMessage> Conversation { get; set; } = [];
}

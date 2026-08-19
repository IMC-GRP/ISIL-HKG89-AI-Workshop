namespace AI_Workshop.UI.Services.Models;

public sealed class IdeaAssistantChatRequest
{
    public string Message { get; set; } = string.Empty;

    public string? ConversationId { get; set; }

    public IdeaAssistantFormSuggestion? CurrentForm { get; set; }

    public IReadOnlyCollection<IdeaAssistantConversationMessage> Conversation { get; set; } = [];
}

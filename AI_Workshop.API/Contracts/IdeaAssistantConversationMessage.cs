namespace AI_Workshop.API.Contracts;

public sealed class IdeaAssistantConversationMessage
{
    public string Role { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}

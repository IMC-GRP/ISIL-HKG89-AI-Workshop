using System.ComponentModel.DataAnnotations;

namespace AI_Workshop.API.Contracts;

public sealed class IdeaAssistantChatRequest
{
    [Required(AllowEmptyStrings = false)]
    public string? Message { get; set; }

    public string? ConversationId { get; set; }

    public IdeaAssistantFormSuggestion? CurrentForm { get; set; }

    public IReadOnlyCollection<IdeaAssistantConversationMessage> Conversation { get; set; } = [];
}

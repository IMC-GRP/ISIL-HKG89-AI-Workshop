using AI_Workshop.AI.Models;

namespace AI_Workshop.AI.Interfaces;

public interface IIdeaAssistantService
{
    Task<AiIdeaAssistantChatResponse> ChatAsync(AiIdeaAssistantChatRequest request, CancellationToken cancellationToken = default);
}

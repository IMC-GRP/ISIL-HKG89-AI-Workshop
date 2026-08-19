namespace AI_Workshop.AI.Exceptions;

public sealed class AiAssistantUnavailableException : Exception
{
    public AiAssistantUnavailableException(string message)
        : base(message)
    {
    }
}

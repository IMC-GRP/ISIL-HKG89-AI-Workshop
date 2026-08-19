namespace AI_Workshop.AI.Configuration;

public sealed class AiAgentOptions
{
    public const string SectionName = "AiAgent";

    public string BaseUrl { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-5.6-luna";
    public string AppId { get; set; } = "AI-Workshop";
    public string SubscriptionKey { get; set; } = string.Empty;
}

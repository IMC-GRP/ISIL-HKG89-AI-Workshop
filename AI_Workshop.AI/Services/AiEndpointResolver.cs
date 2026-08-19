namespace AI_Workshop.AI.Services;

internal static class AiEndpointResolver
{
    private const string ChatCompletionsPath = "/v1/chat/completions";

    public static bool TryResolveChatCompletionsUri(string? configuredBaseUrl, out Uri? chatCompletionsUri, out string error)
    {
        chatCompletionsUri = null;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            error = "AiAgent:BaseUrl is empty.";
            return false;
        }

        if (!Uri.TryCreate(configuredBaseUrl.Trim(), UriKind.Absolute, out var configuredUri))
        {
            error = "AiAgent:BaseUrl is not a valid absolute URI.";
            return false;
        }

        var absolute = configuredUri.GetLeftPart(UriPartial.Path).TrimEnd('/');

        if (absolute.EndsWith(ChatCompletionsPath, StringComparison.OrdinalIgnoreCase))
        {
            chatCompletionsUri = new Uri(absolute, UriKind.Absolute);
            return true;
        }

        if (absolute.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
        {
            chatCompletionsUri = new Uri($"{absolute}/chat/completions", UriKind.Absolute);
            return true;
        }

        chatCompletionsUri = new Uri($"{absolute}{ChatCompletionsPath}", UriKind.Absolute);
        return true;
    }

    public static bool TryResolveOpenAiEndpoint(string? configuredBaseUrl, out Uri? openAiEndpoint, out string error)
    {
        openAiEndpoint = null;
        error = string.Empty;

        if (!TryResolveChatCompletionsUri(configuredBaseUrl, out var chatUri, out error) || chatUri is null)
        {
            return false;
        }

        var chatUriText = chatUri.GetLeftPart(UriPartial.Path);
        var v1Base = chatUriText.Substring(0, chatUriText.Length - "/chat/completions".Length).TrimEnd('/') + "/";
        openAiEndpoint = new Uri(v1Base, UriKind.Absolute);

        return true;
    }
}

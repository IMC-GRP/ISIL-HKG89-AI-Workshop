using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AI_Workshop.UI.Services.Models;

namespace AI_Workshop.UI.Services;

public sealed class IdeaAssistantApiClient
{
    private readonly HttpClient _httpClient;

    public IdeaAssistantApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IdeaAssistantChatResponse> ChatAsync(IdeaAssistantChatRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ideas/assistant/chat", request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            var errorMessage = await GetErrorMessageAsync(response, cancellationToken);
            throw new InvalidOperationException(errorMessage);
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<IdeaAssistantChatResponse>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            throw new InvalidOperationException("Idea assistant response was empty.");
        }

        return payload;
    }

    private static async Task<string> GetErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var envelope = await JsonSerializer.DeserializeAsync<ApiErrorEnvelope>(stream, cancellationToken: cancellationToken);

        if (!string.IsNullOrWhiteSpace(envelope?.Message))
        {
            return envelope.Message;
        }

        return "AI assistant is currently unavailable.";
    }

    private sealed class ApiErrorEnvelope
    {
        public string? Message { get; set; }
    }
}

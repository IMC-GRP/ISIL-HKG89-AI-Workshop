using AI_Workshop.AI.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace AI_Workshop.AI.Services;

internal sealed class AiRawConnectivityChecker
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AiAgentOptions> _options;
    private readonly ILogger<AiRawConnectivityChecker> _logger;

    public AiRawConnectivityChecker(
        IHttpClientFactory httpClientFactory,
        IOptions<AiAgentOptions> options,
        ILogger<AiRawConnectivityChecker> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public async Task<bool> ProbeAsync(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        if (!AiEndpointResolver.TryResolveChatCompletionsUri(options.BaseUrl, out var requestUri, out var error) || requestUri is null)
        {
            _logger.LogWarning("AI raw probe skipped: {Reason}", error);
            return false;
        }

        var hasSubscriptionKey = !string.IsNullOrWhiteSpace(options.SubscriptionKey);
        var hasAppId = !string.IsNullOrWhiteSpace(options.AppId);

        _logger.LogInformation(
            "AI raw probe request config. Url: {Url}; Model: {Model}; HasSubscriptionKey: {HasSubscriptionKey}; HasAppId: {HasAppId}",
            requestUri,
            options.Model,
            hasSubscriptionKey,
            hasAppId);

        if (!hasSubscriptionKey)
        {
            _logger.LogWarning("AI raw probe cannot run because AiAgent:SubscriptionKey is empty.");
            return false;
        }

        var client = _httpClientFactory.CreateClient("AiRawChatCompletions");

        var payload = new
        {
            model = options.Model,
            messages = new[]
            {
                new { role = "user", content = "Reply with the single word OK." }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload)
        };

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var sentRequest = response.RequestMessage;
        var hasKeyHeader = sentRequest?.Headers.Contains("Ocp-Apim-Subscription-Key") ?? false;
        var hasAppIdHeader = sentRequest?.Headers.Contains("x-app-id") ?? false;

        _logger.LogInformation(
            "AI raw probe response. Status: {StatusCode}; Url: {Url}; HasOcpHeaderSent: {HasKeyHeader}; HasAppIdHeaderSent: {HasAppIdHeader}; BodyPreview: {BodyPreview}",
            (int)response.StatusCode,
            requestUri,
            hasKeyHeader,
            hasAppIdHeader,
            SafePreview(responseBody));

        return response.IsSuccessStatusCode;
    }

    private static string SafePreview(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        var trimmed = body.Trim();
        return trimmed.Length <= 900 ? trimmed : trimmed[..900];
    }
}

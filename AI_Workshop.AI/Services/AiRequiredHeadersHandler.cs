using AI_Workshop.AI.Configuration;
using Microsoft.Extensions.Options;

namespace AI_Workshop.AI.Services;

internal sealed class AiRequiredHeadersHandler : DelegatingHandler
{
    private readonly IOptionsMonitor<AiAgentOptions> _optionsMonitor;

    public AiRequiredHeadersHandler(IOptionsMonitor<AiAgentOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var options = _optionsMonitor.CurrentValue;

        request.Headers.Remove("Authorization");
        request.Headers.Remove("Ocp-Apim-Subscription-Key");
        request.Headers.Remove("x-app-id");

        if (!string.IsNullOrWhiteSpace(options.SubscriptionKey))
        {
            request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", options.SubscriptionKey);
        }

        if (!string.IsNullOrWhiteSpace(options.AppId))
        {
            request.Headers.TryAddWithoutValidation("x-app-id", options.AppId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

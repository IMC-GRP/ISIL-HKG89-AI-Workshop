using AI_Workshop.AI.Configuration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI_Workshop.AI.Services;

internal sealed class AiConnectivityProbeHostedService : IHostedService
{
    private readonly IOptions<AiAgentOptions> _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AiConnectivityProbeHostedService> _logger;

    public AiConnectivityProbeHostedService(
        IOptions<AiAgentOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<AiConnectivityProbeHostedService> logger)
    {
        _options = options;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        if (string.IsNullOrWhiteSpace(options.BaseUrl) ||
            string.IsNullOrWhiteSpace(options.Model) ||
            string.IsNullOrWhiteSpace(options.AppId))
        {
            _logger.LogInformation("AI connectivity probe skipped because AiAgent configuration is incomplete.");
            return;
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(25));
        using var scope = _scopeFactory.CreateScope();
        var rawChecker = scope.ServiceProvider.GetRequiredService<AiRawConnectivityChecker>();
        var chatClient = scope.ServiceProvider.GetRequiredService<IChatClient>();
        var agent = scope.ServiceProvider.GetRequiredService<ChatClientAgent>();

        var rawOk = await rawChecker.ProbeAsync(timeoutCts.Token);
        if (!rawOk)
        {
            _logger.LogWarning("AI agent-layer probe skipped because raw chat completion probe failed.");
            return;
        }

        try
        {
            var rawResponse = await chatClient.GetResponseAsync(
                [new ChatMessage(ChatRole.User, "Reply with the single word OK.")],
                new ChatOptions { ModelId = options.Model },
                timeoutCts.Token);

            _logger.LogInformation("AI raw connectivity probe succeeded. Model: {Model}; Response: {Response}", options.Model, rawResponse.Text?.Trim());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI raw connectivity probe failed. Model: {Model}", options.Model);
            return;
        }

        try
        {
            var agentResponse = await agent.RunAsync(
                "Reply with the single word OK.",
                session: null,
                options: new ChatClientAgentRunOptions(new ChatOptions { ModelId = options.Model }),
                cancellationToken: timeoutCts.Token);

            _logger.LogInformation("AI agent-layer probe succeeded. Model: {Model}; Response: {Response}", options.Model, agentResponse.Text?.Trim());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI agent-layer probe failed. Model: {Model}", options.Model);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

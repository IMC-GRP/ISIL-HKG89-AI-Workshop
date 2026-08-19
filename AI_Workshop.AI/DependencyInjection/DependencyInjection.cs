using AI_Workshop.AI.Configuration;
using AI_Workshop.AI.Interfaces;
using AI_Workshop.AI.Prompts;
using AI_Workshop.AI.Services;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace AI_Workshop.AI.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdeaAssistant(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiAgentOptions>(configuration.GetSection(AiAgentOptions.SectionName));
        services.AddTransient<AiRequiredHeadersHandler>();

        services.AddHttpClient("AiRawChatCompletions")
            .AddHttpMessageHandler<AiRequiredHeadersHandler>();

        services.AddScoped<AiRawConnectivityChecker>();

        services.AddScoped<IChatClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AiAgentOptions>>().Value;

            if (!AiEndpointResolver.TryResolveOpenAiEndpoint(options.BaseUrl, out var endpoint, out _))
            {
                endpoint = null;
            }

            var openAiOptions = new OpenAIClientOptions();
            if (endpoint is not null)
            {
                openAiOptions.Endpoint = endpoint;
            }

            openAiOptions.AddPolicy(
                new OpenAiRequiredHeadersPolicy(options.SubscriptionKey, options.AppId),
                PipelinePosition.PerCall);

            var chatClient = new ChatClient(options.Model, new ApiKeyCredential("unused-api-key"), openAiOptions);
            return chatClient.AsIChatClient();
        });

        services.AddScoped(serviceProvider =>
        {
            var chatClient = serviceProvider.GetRequiredService<IChatClient>();

            return chatClient.AsAIAgent(
                name: "IdeaAssistantAgent",
                instructions: IdeaAssistantPrompt.SystemInstructions);
        });

        services.AddScoped<IIdeaAssistantService, IdeaAssistantService>();
        services.AddHostedService<AiConnectivityProbeHostedService>();

        return services;
    }
}

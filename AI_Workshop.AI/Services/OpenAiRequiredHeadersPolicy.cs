using System.ClientModel.Primitives;

namespace AI_Workshop.AI.Services;

internal sealed class OpenAiRequiredHeadersPolicy : PipelinePolicy
{
    private readonly string _subscriptionKey;
    private readonly string _appId;

    public OpenAiRequiredHeadersPolicy(string subscriptionKey, string appId)
    {
        _subscriptionKey = subscriptionKey;
        _appId = appId;
    }

    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        ApplyHeaders(message);
        ProcessNext(message, pipeline, currentIndex);
    }

    public override ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        ApplyHeaders(message);
        return ProcessNextAsync(message, pipeline, currentIndex);
    }

    private void ApplyHeaders(PipelineMessage message)
    {
        message.Request.Headers.Remove("Authorization");
        message.Request.Headers.Set("Ocp-Apim-Subscription-Key", _subscriptionKey);
        message.Request.Headers.Set("x-app-id", _appId);
    }
}

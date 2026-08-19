using System.Text.Json;
using System.Text.Json.Serialization;
using AI_Workshop.UI.Services.Models;

namespace AI_Workshop.UI.Services;

public class IdeasApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _httpClient;

    public IdeasApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<IdeaDto>> GetIdeasAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/ideas", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var ideas = await JsonSerializer.DeserializeAsync<List<IdeaDto>>(stream, JsonOptions, cancellationToken);

        return ideas ?? [];
    }

    public async Task<IdeaDto?> GetIdeaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/ideas/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<IdeaDto>(stream, JsonOptions, cancellationToken);
    }

    public async Task<IdeaDto> CreateIdeaAsync(CreateIdeaRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ideas", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var created = await JsonSerializer.DeserializeAsync<IdeaDto>(stream, JsonOptions, cancellationToken);
        if (created is null)
        {
            throw new InvalidOperationException("Idea creation response was empty.");
        }

        return created;
    }
}

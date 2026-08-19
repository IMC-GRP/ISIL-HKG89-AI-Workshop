using System.Text.Json;
using System.Text.Json.Serialization;
using AI_Workshop.UI.Services.Models;

namespace AI_Workshop.UI.Services;

public class ScheduleApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _httpClient;

    public ScheduleApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ScheduleItemDto>> GetScheduleByDayAsync(int dayNumber, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/schedule/day/{dayNumber}", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var scheduleItems = await JsonSerializer.DeserializeAsync<List<ScheduleItemDto>>(stream, JsonOptions, cancellationToken);

        return scheduleItems ?? [];
    }
}

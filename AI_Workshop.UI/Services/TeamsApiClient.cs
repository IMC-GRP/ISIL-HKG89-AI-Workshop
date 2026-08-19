using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

using AI_Workshop.UI.Services.Models;

namespace AI_Workshop.UI.Services;

public class TeamsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _httpClient;

    public TeamsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WorkshopTeamDto>> GetTeamsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/teams", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var teams = await JsonSerializer.DeserializeAsync<List<WorkshopTeamDto>>(stream, JsonOptions, cancellationToken);

        return teams ?? [];
    }

    public async Task<WorkshopTeamDto?> GetTeamByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/teams/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<WorkshopTeamDto>(stream, JsonOptions, cancellationToken);
    }

    public async Task<WorkshopTeamDto?> GetTeamByIdeaIdAsync(int ideaId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/teams/by-idea/{ideaId}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<WorkshopTeamDto>(stream, JsonOptions, cancellationToken);
    }

    public async Task<WorkshopTeamDto?> GetEmployeeTeamAsync(string employeeEmail, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/teams/employee?employeeEmail={Uri.EscapeDataString(employeeEmail)}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<WorkshopTeamDto>(stream, JsonOptions, cancellationToken);
    }

    public Task<TeamMembershipActionResultDto> JoinTeamAsync(JoinTeamRequest request, CancellationToken cancellationToken = default)
    {
        return SendMembershipActionRequestAsync("api/teams/join", request, cancellationToken);
    }

    public Task<TeamMembershipActionResultDto> LeaveTeamAsync(LeaveTeamRequest request, CancellationToken cancellationToken = default)
    {
        return SendMembershipActionRequestAsync("api/teams/leave", request, cancellationToken);
    }

    public Task<TeamMembershipActionResultDto> SwitchTeamAsync(SwitchTeamRequest request, CancellationToken cancellationToken = default)
    {
        return SendMembershipActionRequestAsync("api/teams/switch", request, cancellationToken);
    }

    public async Task<OrganizerDashboardSummaryDto> GetOrganizerDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/organizer/dashboard", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var summary = await JsonSerializer.DeserializeAsync<OrganizerDashboardSummaryDto>(stream, JsonOptions, cancellationToken);
        if (summary is null)
        {
            throw new InvalidOperationException("Organizer dashboard response was empty.");
        }

        return summary;
    }

    public async Task<WorkshopSettingsDto> GetTeamRegistrationSettingsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/organizer/team-registration", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var settings = await JsonSerializer.DeserializeAsync<WorkshopSettingsDto>(stream, JsonOptions, cancellationToken);
        if (settings is null)
        {
            throw new InvalidOperationException("Team registration settings response was empty.");
        }

        return settings;
    }

    public async Task<WorkshopSettingsDto> OpenTeamRegistrationAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("api/organizer/team-registration/open", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var settings = await JsonSerializer.DeserializeAsync<WorkshopSettingsDto>(stream, JsonOptions, cancellationToken);
        if (settings is null)
        {
            throw new InvalidOperationException("Open registration response was empty.");
        }

        return settings;
    }

    public async Task<WorkshopSettingsDto> CloseTeamRegistrationAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("api/organizer/team-registration/close", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var settings = await JsonSerializer.DeserializeAsync<WorkshopSettingsDto>(stream, JsonOptions, cancellationToken);
        if (settings is null)
        {
            throw new InvalidOperationException("Close registration response was empty.");
        }

        return settings;
    }

    public async Task<WorkshopSettingsDto> UpdateTeamRegistrationCloseDateAsync(DateTime? closeDate, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "api/organizer/team-registration/close-date",
            new UpdateTeamRegistrationCloseDateRequest { TeamRegistrationCloseDate = closeDate },
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var settings = await JsonSerializer.DeserializeAsync<WorkshopSettingsDto>(stream, JsonOptions, cancellationToken);
        if (settings is null)
        {
            throw new InvalidOperationException("Update registration close date response was empty.");
        }

        return settings;
    }

    private async Task<TeamMembershipActionResultDto> SendMembershipActionRequestAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, request, JsonOptions, cancellationToken);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<TeamMembershipActionResultDto>(stream, JsonOptions, cancellationToken);
        if (result is null)
        {
            throw new InvalidOperationException("Membership action response was empty.");
        }

        return result;
    }
}
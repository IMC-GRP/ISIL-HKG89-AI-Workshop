using AI_Workshop.API.Contracts;
using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.CloseTeamRegistration;
using AI_Workshop.Application.UseCases.GetOrganizerDashboardSummary;
using AI_Workshop.Application.UseCases.GetWorkshopSettings;
using AI_Workshop.Application.UseCases.OpenTeamRegistration;
using AI_Workshop.Application.UseCases.UpdateTeamRegistrationCloseDate;
using Microsoft.AspNetCore.Mvc;

namespace AI_Workshop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizerController : ControllerBase
{
    private readonly GetOrganizerDashboardSummaryUseCase _getOrganizerDashboardSummaryUseCase;
    private readonly GetWorkshopSettingsUseCase _getWorkshopSettingsUseCase;
    private readonly OpenTeamRegistrationUseCase _openTeamRegistrationUseCase;
    private readonly CloseTeamRegistrationUseCase _closeTeamRegistrationUseCase;
    private readonly UpdateTeamRegistrationCloseDateUseCase _updateTeamRegistrationCloseDateUseCase;

    public OrganizerController(
        GetOrganizerDashboardSummaryUseCase getOrganizerDashboardSummaryUseCase,
        GetWorkshopSettingsUseCase getWorkshopSettingsUseCase,
        OpenTeamRegistrationUseCase openTeamRegistrationUseCase,
        CloseTeamRegistrationUseCase closeTeamRegistrationUseCase,
        UpdateTeamRegistrationCloseDateUseCase updateTeamRegistrationCloseDateUseCase)
    {
        _getOrganizerDashboardSummaryUseCase = getOrganizerDashboardSummaryUseCase;
        _getWorkshopSettingsUseCase = getWorkshopSettingsUseCase;
        _openTeamRegistrationUseCase = openTeamRegistrationUseCase;
        _closeTeamRegistrationUseCase = closeTeamRegistrationUseCase;
        _updateTeamRegistrationCloseDateUseCase = updateTeamRegistrationCloseDateUseCase;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(OrganizerDashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizerDashboardSummaryDto>> GetDashboardSummary()
    {
        var summary = await _getOrganizerDashboardSummaryUseCase.ExecuteAsync();
        return Ok(summary);
    }

    [HttpGet("team-registration")]
    [ProducesResponseType(typeof(WorkshopSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkshopSettingsDto>> GetTeamRegistrationSettings()
    {
        var settings = await _getWorkshopSettingsUseCase.ExecuteAsync();
        return Ok(settings);
    }

    [HttpPost("team-registration/open")]
    [ProducesResponseType(typeof(WorkshopSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkshopSettingsDto>> OpenTeamRegistration()
    {
        var settings = await _openTeamRegistrationUseCase.ExecuteAsync();
        return Ok(settings);
    }

    [HttpPost("team-registration/close")]
    [ProducesResponseType(typeof(WorkshopSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkshopSettingsDto>> CloseTeamRegistration()
    {
        var settings = await _closeTeamRegistrationUseCase.ExecuteAsync();
        return Ok(settings);
    }

    [HttpPut("team-registration/close-date")]
    [ProducesResponseType(typeof(WorkshopSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkshopSettingsDto>> UpdateTeamRegistrationCloseDate([FromBody] UpdateTeamRegistrationCloseDateRequest request)
    {
        var settings = await _updateTeamRegistrationCloseDateUseCase.ExecuteAsync(new UpdateTeamRegistrationCloseDateRequestDto
        {
            TeamRegistrationCloseDate = request.TeamRegistrationCloseDate
        });

        return Ok(settings);
    }
}

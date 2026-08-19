using AI_Workshop.API.Contracts;
using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.GetEmployeeWorkshopTeam;
using AI_Workshop.Application.UseCases.GetWorkshopTeamById;
using AI_Workshop.Application.UseCases.GetWorkshopTeamByIdeaId;
using AI_Workshop.Application.UseCases.GetWorkshopTeams;
using AI_Workshop.Application.UseCases.JoinWorkshopTeam;
using AI_Workshop.Application.UseCases.LeaveWorkshopTeam;
using AI_Workshop.Application.UseCases.SwitchWorkshopTeam;
using Microsoft.AspNetCore.Mvc;

namespace AI_Workshop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly GetWorkshopTeamsUseCase _getWorkshopTeamsUseCase;
    private readonly GetWorkshopTeamByIdUseCase _getWorkshopTeamByIdUseCase;
    private readonly GetWorkshopTeamByIdeaIdUseCase _getWorkshopTeamByIdeaIdUseCase;
    private readonly GetEmployeeWorkshopTeamUseCase _getEmployeeWorkshopTeamUseCase;
    private readonly JoinWorkshopTeamUseCase _joinWorkshopTeamUseCase;
    private readonly LeaveWorkshopTeamUseCase _leaveWorkshopTeamUseCase;
    private readonly SwitchWorkshopTeamUseCase _switchWorkshopTeamUseCase;

    public TeamsController(
        GetWorkshopTeamsUseCase getWorkshopTeamsUseCase,
        GetWorkshopTeamByIdUseCase getWorkshopTeamByIdUseCase,
        GetWorkshopTeamByIdeaIdUseCase getWorkshopTeamByIdeaIdUseCase,
        GetEmployeeWorkshopTeamUseCase getEmployeeWorkshopTeamUseCase,
        JoinWorkshopTeamUseCase joinWorkshopTeamUseCase,
        LeaveWorkshopTeamUseCase leaveWorkshopTeamUseCase,
        SwitchWorkshopTeamUseCase switchWorkshopTeamUseCase)
    {
        _getWorkshopTeamsUseCase = getWorkshopTeamsUseCase;
        _getWorkshopTeamByIdUseCase = getWorkshopTeamByIdUseCase;
        _getWorkshopTeamByIdeaIdUseCase = getWorkshopTeamByIdeaIdUseCase;
        _getEmployeeWorkshopTeamUseCase = getEmployeeWorkshopTeamUseCase;
        _joinWorkshopTeamUseCase = joinWorkshopTeamUseCase;
        _leaveWorkshopTeamUseCase = leaveWorkshopTeamUseCase;
        _switchWorkshopTeamUseCase = switchWorkshopTeamUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<WorkshopTeamDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<WorkshopTeamDto>>> GetTeams()
    {
        var teams = await _getWorkshopTeamsUseCase.ExecuteAsync();
        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkshopTeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkshopTeamDto>> GetTeamById(int id)
    {
        var team = await _getWorkshopTeamByIdUseCase.ExecuteAsync(id);
        if (team is null)
        {
            return NotFound();
        }

        return Ok(team);
    }

    [HttpGet("by-idea/{ideaId:int}")]
    [ProducesResponseType(typeof(WorkshopTeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkshopTeamDto>> GetTeamByIdeaId(int ideaId)
    {
        var team = await _getWorkshopTeamByIdeaIdUseCase.ExecuteAsync(ideaId);
        if (team is null)
        {
            return NotFound();
        }

        return Ok(team);
    }

    [HttpGet("employee")]
    [ProducesResponseType(typeof(WorkshopTeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkshopTeamDto>> GetEmployeeTeam([FromQuery] string employeeEmail)
    {
        if (string.IsNullOrWhiteSpace(employeeEmail))
        {
            return BadRequest("Employee email is required.");
        }

        var team = await _getEmployeeWorkshopTeamUseCase.ExecuteAsync(employeeEmail);
        if (team is null)
        {
            return NotFound();
        }

        return Ok(team);
    }

    [HttpPost("join")]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TeamMembershipActionResultDto>> JoinTeam([FromBody] JoinTeamRequest request)
    {
        var result = await _joinWorkshopTeamUseCase.ExecuteAsync(new JoinTeamRequestDto
        {
            TeamId = request.TeamId,
            EmployeeName = request.EmployeeName?.Trim() ?? string.Empty,
            EmployeeEmail = request.EmployeeEmail?.Trim() ?? string.Empty
        });

        return ToMembershipActionResult(result);
    }

    [HttpPost("leave")]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TeamMembershipActionResultDto>> LeaveTeam([FromBody] LeaveTeamRequest request)
    {
        var result = await _leaveWorkshopTeamUseCase.ExecuteAsync(request.TeamId, request.EmployeeEmail?.Trim() ?? string.Empty);
        return ToMembershipActionResult(result);
    }

    [HttpPost("switch")]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TeamMembershipActionResultDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TeamMembershipActionResultDto>> SwitchTeam([FromBody] SwitchTeamRequest request)
    {
        var result = await _switchWorkshopTeamUseCase.ExecuteAsync(new SwitchTeamRequestDto
        {
            TargetTeamId = request.TargetTeamId,
            EmployeeName = request.EmployeeName?.Trim() ?? string.Empty,
            EmployeeEmail = request.EmployeeEmail?.Trim() ?? string.Empty
        });

        return ToMembershipActionResult(result);
    }

    private ActionResult<TeamMembershipActionResultDto> ToMembershipActionResult(TeamMembershipActionResultDto result)
    {
        if (result.Succeeded)
        {
            return Ok(result);
        }

        return result.Code switch
        {
            "validation_failed" => BadRequest(result),
            "team_not_found" or "target_team_not_found" => NotFound(result),
            "member_not_found" => NotFound(result),
            _ => Conflict(result)
        };
    }
}
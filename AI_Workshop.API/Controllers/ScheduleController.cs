using AI_Workshop.Application.DTOs;
using AI_Workshop.Application.UseCases.GetSchedule;
using Microsoft.AspNetCore.Mvc;

namespace AI_Workshop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly GetScheduleUseCase _getScheduleUseCase;
    private readonly GetScheduleByDayUseCase _getScheduleByDayUseCase;

    public ScheduleController(
        GetScheduleUseCase getScheduleUseCase,
        GetScheduleByDayUseCase getScheduleByDayUseCase)
    {
        _getScheduleUseCase = getScheduleUseCase;
        _getScheduleByDayUseCase = getScheduleByDayUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ScheduleItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ScheduleItemDto>>> GetSchedule()
    {
        var schedule = await _getScheduleUseCase.ExecuteAsync();
        return Ok(schedule);
    }

    [HttpGet("day/{dayNumber:int}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ScheduleItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<ScheduleItemDto>>> GetScheduleByDay(int dayNumber)
    {
        if (dayNumber <= 0)
        {
            return BadRequest();
        }

        var schedule = await _getScheduleByDayUseCase.ExecuteAsync(dayNumber);
        return Ok(schedule);
    }
}

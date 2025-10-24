using Microsoft.AspNetCore.Mvc;
using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;

namespace Doctorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AttendeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Accept or reject event invitation
    /// </summary>
    /// <param name="id">Attendee ID</param>
    /// <param name="command">Status update details</param>
    /// <returns>Updated attendee</returns>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<AttendeeDto>> UpdateAttendeeStatus(Guid id, [FromBody] UpdateAttendeeStatusCommand command)
    {
        if (id != command.AttendeeId)
            return BadRequest("ID in URL does not match ID in request body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var attendeeDto = await _mediator.Send(command);

        if (attendeeDto == null)
            return NotFound($"Attendee with ID {id} not found.");

        return Ok(attendeeDto);
    }
}
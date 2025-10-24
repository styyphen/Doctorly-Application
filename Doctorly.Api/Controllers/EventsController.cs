using Microsoft.AspNetCore.Mvc;
using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.Queries;
using HealthApp.Application.DTOs;

namespace Doctorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all events with optional filtering
    /// </summary>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="searchTerm">Search in title and description</param>
    /// <returns>List of events</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetEventsQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            SearchTerm = searchTerm
        };

        var events = await _mediator.Send(query);
        return Ok(events);
    }

    /// <summary>
    /// Get a specific event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Event details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<EventDto>> GetEvent(Guid id)
    {
        var query = new GetEventByIdQuery { Id = id };
        var eventDto = await _mediator.Send(query);

        if (eventDto == null)
            return NotFound($"Event with ID {id} not found.");

        return Ok(eventDto);
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    /// <param name="command">Event creation details</param>
    /// <returns>Created event</returns>
    [HttpPost]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var eventDto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEvent), new { id = eventDto.Id }, eventDto);
    }

    /// <summary>
    /// Update an existing event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="command">Event update details</param>
    /// <returns>Updated event</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<EventDto>> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID in URL does not match ID in request body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var eventDto = await _mediator.Send(command);

        if (eventDto == null)
            return NotFound($"Event with ID {id} not found.");

        return Ok(eventDto);
    }

    /// <summary>
    /// Delete an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(Guid id)
    {
        var command = new DeleteEventCommand { Id = id };
        var success = await _mediator.Send(command);

        if (!success)
            return NotFound($"Event with ID {id} not found.");

        return NoContent();
    }
}
using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;

namespace Doctorly.Api.GraphQL.Mutations;

public class EventMutations
{
    private readonly IMediator _mediator;

    public EventMutations(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<EventDto> CreateEvent(CreateEventCommand input)
    {
        return await _mediator.Send(input);
    }

    public async Task<EventDto?> UpdateEvent(UpdateEventCommand input)
    {
        return await _mediator.Send(input);
    }

    public async Task<bool> DeleteEvent(DeleteEventCommand input)
    {
        return await _mediator.Send(input);
    }
}
using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;

namespace Doctorly.Api.GraphQL.Mutations;

public class AttendeeMutations
{
    private readonly IMediator _mediator;

    public AttendeeMutations(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<AttendeeDto?> UpdateAttendeeStatus(UpdateAttendeeStatusCommand input)
    {
        return await _mediator.Send(input);
    }
}
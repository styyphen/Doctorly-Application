using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;

namespace Doctorly.Api.GraphQL.Mutations;

public class PatientMutations
{
    private readonly IMediator _mediator;

    public PatientMutations(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<PatientDto> CreatePatient(CreatePatientDto input)
    {
        return await _mediator.Send(new CreatePatientCommand(input));
    }
}
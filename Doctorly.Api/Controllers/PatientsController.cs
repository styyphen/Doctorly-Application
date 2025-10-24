using Microsoft.AspNetCore.Mvc;
using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.Queries;
using HealthApp.Application.DTOs;

namespace Doctorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
    {
        var patients = await _mediator.Send(new GetAllPatientsQuery());
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        var patient = await _mediator.Send(new GetPatientByIdQuery(id));

        if (patient == null)
            return NotFound();

        return Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientDto patientDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var patient = await _mediator.Send(new CreatePatientCommand(patientDto));
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }
}
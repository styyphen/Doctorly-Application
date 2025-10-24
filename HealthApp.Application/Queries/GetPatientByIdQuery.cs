using MediatR;
using HealthApp.Application.DTOs;

namespace HealthApp.Application.Queries;

public record GetPatientByIdQuery(Guid PatientId) : IRequest<PatientDto?>;

public record GetAllPatientsQuery() : IRequest<IEnumerable<PatientDto>>;
using MediatR;
using HealthApp.Application.DTOs;

namespace HealthApp.Application.Commands;

public record CreatePatientCommand(CreatePatientDto PatientDto) : IRequest<PatientDto>;
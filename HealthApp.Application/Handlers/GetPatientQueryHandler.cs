using MediatR;
using HealthApp.Application.Queries;
using HealthApp.Application.DTOs;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Interfaces;

namespace HealthApp.Application.Handlers;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
{
    private readonly IRepository<Patient> _patientRepository;

    public GetPatientByIdQueryHandler(IRepository<Patient> patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);

        if (patient == null)
            return null;

        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Address = patient.Address,
            EmergencyContact = patient.EmergencyContact,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt
        };
    }
}

public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, IEnumerable<PatientDto>>
{
    private readonly IRepository<Patient> _patientRepository;

    public GetAllPatientsQueryHandler(IRepository<Patient> patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.GetAllAsync();

        return patients.Select(patient => new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Address = patient.Address,
            EmergencyContact = patient.EmergencyContact,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt
        });
    }
}
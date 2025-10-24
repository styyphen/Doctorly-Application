using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Interfaces;

namespace HealthApp.Application.Handlers;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
{
    private readonly IRepository<Patient> _patientRepository;

    public CreatePatientCommandHandler(IRepository<Patient> patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.PatientDto.FirstName,
            LastName = request.PatientDto.LastName,
            Email = request.PatientDto.Email,
            PhoneNumber = request.PatientDto.PhoneNumber,
            DateOfBirth = request.PatientDto.DateOfBirth,
            Gender = request.PatientDto.Gender,
            Address = request.PatientDto.Address,
            EmergencyContact = request.PatientDto.EmergencyContact,
            EmergencyContactPhone = request.PatientDto.EmergencyContactPhone
        };

        await _patientRepository.AddAsync(patient);

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
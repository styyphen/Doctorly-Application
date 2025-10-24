using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using HealthApp.Infrastructure.Data;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using Bogus;

namespace HealthApp.Infrastructure.Tests.Data;

public class HealthAppDbContextTests : IDisposable
{
    private readonly HealthAppDbContext _context;
    private readonly Faker<Patient> _patientFaker;
    private readonly Faker<Doctor> _doctorFaker;

    public HealthAppDbContextTests()
    {
        var options = new DbContextOptionsBuilder<HealthAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new HealthAppDbContext(options);

        _patientFaker = new Faker<Patient>()
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(80, DateTime.Now.AddYears(-18)))
            .RuleFor(p => p.Gender, f => f.PickRandom<Gender>())
            .RuleFor(p => p.Address, f => f.Address.FullAddress())
            .RuleFor(p => p.EmergencyContact, f => f.Name.FullName())
            .RuleFor(p => p.EmergencyContactPhone, f => f.Phone.PhoneNumber());

        _doctorFaker = new Faker<Doctor>()
            .RuleFor(d => d.Id, f => f.Random.Guid())
            .RuleFor(d => d.FirstName, f => f.Name.FirstName())
            .RuleFor(d => d.LastName, f => f.Name.LastName())
            .RuleFor(d => d.Email, f => f.Internet.Email())
            .RuleFor(d => d.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(d => d.Specialization, f => f.PickRandom("Cardiology", "Neurology", "Orthopedics", "Pediatrics"))
            .RuleFor(d => d.LicenseNumber, f => f.Random.AlphaNumeric(10))
            .RuleFor(d => d.Department, f => f.PickRandom("Emergency", "Surgery", "Internal Medicine"));
    }

    [Fact]
    public async Task SaveChanges_Should_Set_CreatedAt_For_New_Entities()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        var beforeTime = DateTime.UtcNow;

        // Act
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        var afterTime = DateTime.UtcNow;

        // Assert
        patient.CreatedAt.Should().BeAfter(beforeTime.AddSeconds(-1));
        patient.CreatedAt.Should().BeBefore(afterTime.AddSeconds(1));
        patient.UpdatedAt.Should().BeAfter(beforeTime.AddSeconds(-1));
        patient.UpdatedAt.Should().BeBefore(afterTime.AddSeconds(1));
    }

    [Fact]
    public async Task SaveChanges_Should_Update_UpdatedAt_For_Modified_Entities()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var originalCreatedAt = patient.CreatedAt;
        var originalUpdatedAt = patient.UpdatedAt;
        await Task.Delay(10); // Ensure timestamp difference

        // Act
        patient.FirstName = "UpdatedName";
        await _context.SaveChangesAsync();

        // Assert
        patient.CreatedAt.Should().Be(originalCreatedAt); // Should not change
        patient.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact(Skip = "InMemory database doesn't enforce unique constraints")]
    public async Task Patient_Email_Should_Be_Unique()
    {
        // Arrange
        var email = "test@example.com";
        var patient1 = _patientFaker.Generate();
        var patient2 = _patientFaker.Generate();
        patient1.Email = email;
        patient2.Email = email;

        _context.Patients.Add(patient1);
        await _context.SaveChangesAsync();

        // Act & Assert
        _context.Patients.Add(patient2);
        var act = async () => await _context.SaveChangesAsync();
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact(Skip = "InMemory database doesn't enforce unique constraints")]
    public async Task Doctor_Email_Should_Be_Unique()
    {
        // Arrange
        var email = "doctor@example.com";
        var doctor1 = _doctorFaker.Generate();
        var doctor2 = _doctorFaker.Generate();
        doctor1.Email = email;
        doctor2.Email = email;

        _context.Doctors.Add(doctor1);
        await _context.SaveChangesAsync();

        // Act & Assert
        _context.Doctors.Add(doctor2);
        var act = async () => await _context.SaveChangesAsync();
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact(Skip = "InMemory database doesn't enforce unique constraints")]
    public async Task Doctor_LicenseNumber_Should_Be_Unique()
    {
        // Arrange
        var licenseNumber = "LIC123456";
        var doctor1 = _doctorFaker.Generate();
        var doctor2 = _doctorFaker.Generate();
        doctor1.LicenseNumber = licenseNumber;
        doctor2.LicenseNumber = licenseNumber;

        _context.Doctors.Add(doctor1);
        await _context.SaveChangesAsync();

        // Act & Assert
        _context.Doctors.Add(doctor2);
        var act = async () => await _context.SaveChangesAsync();
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Appointment_Should_Have_Proper_Relationships()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        var doctor = _doctorFaker.Generate();
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Reason = "Routine checkup",
            Status = AppointmentStatus.Scheduled
        };

        // Act
        _context.Patients.Add(patient);
        _context.Doctors.Add(doctor);
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Assert
        var savedAppointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstAsync(a => a.Id == appointment.Id);

        savedAppointment.Patient.Should().NotBeNull();
        savedAppointment.Patient.Id.Should().Be(patient.Id);
        savedAppointment.Doctor.Should().NotBeNull();
        savedAppointment.Doctor.Id.Should().Be(doctor.Id);
    }

    [Fact]
    public async Task MedicalRecord_Should_Have_Proper_Relationships()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        var doctor = _doctorFaker.Generate();
        var medicalRecord = new MedicalRecord
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            VisitDate = DateTime.UtcNow,
            Diagnosis = "Test diagnosis",
            Treatment = "Test treatment",
            Medications = "Test medications",
            Notes = "Test notes"
        };

        // Act
        _context.Patients.Add(patient);
        _context.Doctors.Add(doctor);
        _context.MedicalRecords.Add(medicalRecord);
        await _context.SaveChangesAsync();

        // Assert
        var savedRecord = await _context.MedicalRecords
            .Include(mr => mr.Patient)
            .Include(mr => mr.Doctor)
            .FirstAsync(mr => mr.Id == medicalRecord.Id);

        savedRecord.Patient.Should().NotBeNull();
        savedRecord.Patient.Id.Should().Be(patient.Id);
        savedRecord.Doctor.Should().NotBeNull();
        savedRecord.Doctor.Id.Should().Be(doctor.Id);
    }

    [Fact]
    public async Task Patient_Should_Support_Multiple_Appointments()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        var doctor = _doctorFaker.Generate();
        var appointment1 = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Status = AppointmentStatus.Scheduled
        };
        var appointment2 = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(2),
            Status = AppointmentStatus.Scheduled
        };

        // Act
        _context.Patients.Add(patient);
        _context.Doctors.Add(doctor);
        _context.Appointments.AddRange(appointment1, appointment2);
        await _context.SaveChangesAsync();

        // Assert
        var savedPatient = await _context.Patients
            .Include(p => p.Appointments)
            .FirstAsync(p => p.Id == patient.Id);

        savedPatient.Appointments.Should().HaveCount(2);
        savedPatient.Appointments.Should().Contain(a => a.Id == appointment1.Id);
        savedPatient.Appointments.Should().Contain(a => a.Id == appointment2.Id);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
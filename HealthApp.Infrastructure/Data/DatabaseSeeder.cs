using Microsoft.EntityFrameworkCore;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;

namespace HealthApp.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(HealthAppDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Patients.AnyAsync() || await context.Doctors.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Doctors
        var doctors = new List<Doctor>
        {
            new Doctor
            {
                Id = Guid.NewGuid(),
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@doctorly.com",
                PhoneNumber = "+1-555-0101",
                Specialization = "Cardiology",
                LicenseNumber = "MD001234",
                YearsOfExperience = 15,
                Department = "Cardiology",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FirstName = "Michael",
                LastName = "Chen",
                Email = "michael.chen@doctorly.com",
                PhoneNumber = "+1-555-0102",
                Specialization = "Pediatrics",
                LicenseNumber = "MD005678",
                YearsOfExperience = 8,
                Department = "Pediatrics",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FirstName = "Emily",
                LastName = "Rodriguez",
                Email = "emily.rodriguez@doctorly.com",
                PhoneNumber = "+1-555-0103",
                Specialization = "Dermatology",
                LicenseNumber = "MD009012",
                YearsOfExperience = 12,
                Department = "Dermatology",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FirstName = "David",
                LastName = "Williams",
                Email = "david.williams@doctorly.com",
                PhoneNumber = "+1-555-0104",
                Specialization = "Orthopedics",
                LicenseNumber = "MD003456",
                YearsOfExperience = 20,
                Department = "Orthopedics",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FirstName = "Lisa",
                LastName = "Thompson",
                Email = "lisa.thompson@doctorly.com",
                PhoneNumber = "+1-555-0105",
                Specialization = "General Practice",
                LicenseNumber = "MD007890",
                YearsOfExperience = 6,
                Department = "General Medicine",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Doctors.AddRangeAsync(doctors);

        // Seed Patients
        var patients = new List<Patient>
        {
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@email.com",
                PhoneNumber = "+1-555-1001",
                DateOfBirth = new DateTime(1985, 3, 15),
                Gender = Gender.Male,
                Address = "123 Main St, Anytown, ST 12345",
                EmergencyContact = "Jane Smith",
                EmergencyContactPhone = "+1-555-1002",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Maria",
                LastName = "Garcia",
                Email = "maria.garcia@email.com",
                PhoneNumber = "+1-555-1003",
                DateOfBirth = new DateTime(1992, 7, 22),
                Gender = Gender.Female,
                Address = "456 Oak Ave, Somewhere, ST 67890",
                EmergencyContact = "Carlos Garcia",
                EmergencyContactPhone = "+1-555-1004",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Robert",
                LastName = "Johnson",
                Email = "robert.johnson@email.com",
                PhoneNumber = "+1-555-1005",
                DateOfBirth = new DateTime(1978, 11, 8),
                Gender = Gender.Male,
                Address = "789 Pine St, Elsewhere, ST 11111",
                EmergencyContact = "Susan Johnson",
                EmergencyContactPhone = "+1-555-1006",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Sarah",
                LastName = "Wilson",
                Email = "sarah.wilson@email.com",
                PhoneNumber = "+1-555-1007",
                DateOfBirth = new DateTime(1995, 5, 12),
                Gender = Gender.Female,
                Address = "321 Elm Dr, Nowhere, ST 22222",
                EmergencyContact = "Tom Wilson",
                EmergencyContactPhone = "+1-555-1008",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Taylor",
                Email = "alex.taylor@email.com",
                PhoneNumber = "+1-555-1009",
                DateOfBirth = new DateTime(1988, 9, 3),
                Gender = Gender.Other,
                Address = "654 Maple Ln, Anywhere, ST 33333",
                EmergencyContact = "Jordan Taylor",
                EmergencyContactPhone = "+1-555-1010",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Patients.AddRangeAsync(patients);

        // Save doctors and patients first to get their IDs
        await context.SaveChangesAsync();

        // Seed Appointments
        var appointments = new List<Appointment>
        {
            new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patients[0].Id,
                DoctorId = doctors[0].Id,
                ScheduledDateTime = DateTime.UtcNow.AddDays(1).AddHours(9),
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled,
                Reason = "Annual checkup",
                Notes = "Regular health screening",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patients[1].Id,
                DoctorId = doctors[1].Id,
                ScheduledDateTime = DateTime.UtcNow.AddDays(2).AddHours(14),
                DurationMinutes = 45,
                Status = AppointmentStatus.Scheduled,
                Reason = "Vaccination",
                Notes = "Annual flu shot",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patients[2].Id,
                DoctorId = doctors[2].Id,
                ScheduledDateTime = DateTime.UtcNow.AddDays(3).AddHours(10),
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled,
                Reason = "Skin consultation",
                Notes = "Mole examination",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patients[3].Id,
                DoctorId = doctors[3].Id,
                ScheduledDateTime = DateTime.UtcNow.AddDays(-1).AddHours(15),
                DurationMinutes = 60,
                Status = AppointmentStatus.Completed,
                Reason = "Knee pain assessment",
                Notes = "Physical therapy recommended",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Appointments.AddRangeAsync(appointments);

        // Seed Medical Records
        var medicalRecords = new List<MedicalRecord>
        {
            new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = patients[0].Id,
                DoctorId = doctors[0].Id,
                Diagnosis = "Hypertension",
                Treatment = "Lifestyle modifications and medication",
                Medications = "Lisinopril 10mg daily",
                Notes = "Patient advised to monitor blood pressure daily",
                VisitDate = DateTime.UtcNow.AddDays(-30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = patients[3].Id,
                DoctorId = doctors[3].Id,
                Diagnosis = "Mild osteoarthritis in right knee",
                Treatment = "Physical therapy and anti-inflammatory medication",
                Medications = "Ibuprofen 400mg as needed",
                Notes = "Patient responded well to treatment. Follow up in 6 weeks",
                VisitDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.MedicalRecords.AddRangeAsync(medicalRecords);

        // Save all changes
        await context.SaveChangesAsync();
    }
}
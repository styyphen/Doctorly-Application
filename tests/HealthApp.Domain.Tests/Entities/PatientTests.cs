using FluentAssertions;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;

namespace HealthApp.Domain.Tests.Entities;

public class PatientTests
{
    [Fact]
    public void Patient_Should_Initialize_Collections()
    {
        // Arrange & Act
        var patient = new Patient();

        // Assert
        patient.Appointments.Should().NotBeNull().And.BeEmpty();
        patient.MedicalRecords.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Patient_Should_Set_Properties_Correctly()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "555-0123";
        var dateOfBirth = new DateTime(1990, 1, 1);
        var gender = Gender.Male;
        var address = "123 Main St";
        var emergencyContact = "Jane Doe";
        var emergencyContactPhone = "555-0124";

        // Act
        var patient = new Patient
        {
            Id = patientId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            Address = address,
            EmergencyContact = emergencyContact,
            EmergencyContactPhone = emergencyContactPhone
        };

        // Assert
        patient.Id.Should().Be(patientId);
        patient.FirstName.Should().Be(firstName);
        patient.LastName.Should().Be(lastName);
        patient.Email.Should().Be(email);
        patient.PhoneNumber.Should().Be(phoneNumber);
        patient.DateOfBirth.Should().Be(dateOfBirth);
        patient.Gender.Should().Be(gender);
        patient.Address.Should().Be(address);
        patient.EmergencyContact.Should().Be(emergencyContact);
        patient.EmergencyContactPhone.Should().Be(emergencyContactPhone);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Patient_Should_Allow_Empty_Optional_Fields(string emptyValue)
    {
        // Arrange & Act
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            Address = emptyValue,
            EmergencyContact = emptyValue,
            EmergencyContactPhone = emptyValue
        };

        // Assert
        patient.FirstName.Should().Be("John");
        patient.LastName.Should().Be("Doe");
        patient.Address.Should().Be(emptyValue);
        patient.EmergencyContact.Should().Be(emptyValue);
        patient.EmergencyContactPhone.Should().Be(emptyValue);
    }

    [Fact]
    public void Patient_Should_Support_Multiple_Appointments()
    {
        // Arrange
        var patient = new Patient { Id = Guid.NewGuid() };
        var appointment1 = new Appointment { Id = Guid.NewGuid(), PatientId = patient.Id };
        var appointment2 = new Appointment { Id = Guid.NewGuid(), PatientId = patient.Id };

        // Act
        patient.Appointments.Add(appointment1);
        patient.Appointments.Add(appointment2);

        // Assert
        patient.Appointments.Should().HaveCount(2);
        patient.Appointments.Should().Contain(appointment1);
        patient.Appointments.Should().Contain(appointment2);
    }

    [Fact]
    public void Patient_Should_Support_Multiple_MedicalRecords()
    {
        // Arrange
        var patient = new Patient { Id = Guid.NewGuid() };
        var record1 = new MedicalRecord { Id = Guid.NewGuid(), PatientId = patient.Id };
        var record2 = new MedicalRecord { Id = Guid.NewGuid(), PatientId = patient.Id };

        // Act
        patient.MedicalRecords.Add(record1);
        patient.MedicalRecords.Add(record2);

        // Assert
        patient.MedicalRecords.Should().HaveCount(2);
        patient.MedicalRecords.Should().Contain(record1);
        patient.MedicalRecords.Should().Contain(record2);
    }

    [Fact]
    public void Patient_Should_Support_All_Gender_Values()
    {
        // Arrange & Act & Assert
        var malePat = new Patient { Gender = Gender.Male };
        malePat.Gender.Should().Be(Gender.Male);

        var femalePat = new Patient { Gender = Gender.Female };
        femalePat.Gender.Should().Be(Gender.Female);

        var otherPat = new Patient { Gender = Gender.Other };
        otherPat.Gender.Should().Be(Gender.Other);

        var preferNotPat = new Patient { Gender = Gender.PreferNotToSay };
        preferNotPat.Gender.Should().Be(Gender.PreferNotToSay);
    }
}
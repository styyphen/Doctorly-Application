using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using HealthApp.Infrastructure.Data;
using HealthApp.Infrastructure.Repositories;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using Bogus;

namespace HealthApp.Infrastructure.Tests.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly HealthAppDbContext _context;
    private readonly Repository<Patient> _repository;
    private readonly Faker<Patient> _patientFaker;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<HealthAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new HealthAppDbContext(options);
        _repository = new Repository<Patient>(_context);

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
    }

    [Fact]
    public async Task AddAsync_Should_Add_Patient_And_Return_Entity()
    {
        // Arrange
        var patient = _patientFaker.Generate();

        // Act
        var result = await _repository.AddAsync(patient);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(patient.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedPatient = await _context.Patients.FindAsync(patient.Id);
        savedPatient.Should().NotBeNull();
        savedPatient!.FirstName.Should().Be(patient.FirstName);
        savedPatient.LastName.Should().Be(patient.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Patient_When_Exists()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(patient.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(patient.Id);
        result.FirstName.Should().Be(patient.FirstName);
        result.LastName.Should().Be(patient.LastName);
        result.Email.Should().Be(patient.Email);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Patients()
    {
        // Arrange
        var patients = _patientFaker.Generate(3);
        await _context.Patients.AddRangeAsync(patients);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainEquivalentOf(patients[0]);
        result.Should().ContainEquivalentOf(patients[1]);
        result.Should().ContainEquivalentOf(patients[2]);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Patients()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_Should_Return_Matching_Patients()
    {
        // Arrange
        var patients = _patientFaker.Generate(5);
        patients[0].FirstName = "John";
        patients[1].FirstName = "John";
        patients[2].FirstName = "Jane";
        patients[3].FirstName = "Bob";
        patients[4].FirstName = "Alice";

        await _context.Patients.AddRangeAsync(patients);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(p => p.FirstName == "John");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.FirstName == "John");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Patient()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        var originalUpdatedAt = patient.UpdatedAt;
        await Task.Delay(10); // Ensure timestamp difference

        // Act
        patient.FirstName = "UpdatedName";
        await _repository.UpdateAsync(patient);

        // Assert
        var updatedPatient = await _context.Patients.FindAsync(patient.Id);
        updatedPatient.Should().NotBeNull();
        updatedPatient!.FirstName.Should().Be("UpdatedName");
        updatedPatient.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Patient()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(patient);

        // Assert
        var deletedPatient = await _context.Patients.FindAsync(patient.Id);
        deletedPatient.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_Patient_Exists()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(patient.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_False_When_Patient_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task FindAsync_Should_Return_Empty_When_No_Matches()
    {
        // Arrange
        var patients = _patientFaker.Generate(3);
        await _context.Patients.AddRangeAsync(patients);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(p => p.FirstName == "NonExistentName");

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
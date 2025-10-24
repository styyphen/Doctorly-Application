using FluentAssertions;
using HealthApp.Application.Queries;
using HealthApp.Application.Handlers;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using HealthApp.Domain.Interfaces;
using Moq;
using Bogus;

namespace HealthApp.Application.Tests.Queries;

public class GetPatientByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Patient>> _mockRepository;
    private readonly GetPatientByIdQueryHandler _handler;
    private readonly Faker<Patient> _patientFaker;

    public GetPatientByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IRepository<Patient>>();
        _handler = new GetPatientByIdQueryHandler(_mockRepository.Object);

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
            .RuleFor(p => p.EmergencyContactPhone, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent())
            .RuleFor(p => p.UpdatedAt, f => f.Date.Recent());
    }

    [Fact]
    public async Task Handle_Should_Return_PatientDto_When_Patient_Exists()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        var query = new GetPatientByIdQuery(patient.Id);

        _mockRepository.Setup(x => x.GetByIdAsync(patient.Id))
            .ReturnsAsync(patient);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(patient.Id);
        result.FirstName.Should().Be(patient.FirstName);
        result.LastName.Should().Be(patient.LastName);
        result.Email.Should().Be(patient.Email);
        result.PhoneNumber.Should().Be(patient.PhoneNumber);
        result.DateOfBirth.Should().Be(patient.DateOfBirth);
        result.Gender.Should().Be(patient.Gender);
        result.Address.Should().Be(patient.Address);
        result.EmergencyContact.Should().Be(patient.EmergencyContact);
        result.EmergencyContactPhone.Should().Be(patient.EmergencyContactPhone);
        result.CreatedAt.Should().Be(patient.CreatedAt);
        result.UpdatedAt.Should().Be(patient.UpdatedAt);

        _mockRepository.Verify(x => x.GetByIdAsync(patient.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Patient_Not_Found()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var query = new GetPatientByIdQuery(patientId);

        _mockRepository.Setup(x => x.GetByIdAsync(patientId))
            .ReturnsAsync((Patient?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(patientId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_With_Correct_Id()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var query = new GetPatientByIdQuery(patientId);

        _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Patient?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.GetByIdAsync(patientId), Times.Once);
    }
}
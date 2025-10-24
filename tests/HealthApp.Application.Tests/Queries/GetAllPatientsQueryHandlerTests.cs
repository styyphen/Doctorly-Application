using FluentAssertions;
using HealthApp.Application.Queries;
using HealthApp.Application.Handlers;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using HealthApp.Domain.Interfaces;
using Moq;
using Bogus;

namespace HealthApp.Application.Tests.Queries;

public class GetAllPatientsQueryHandlerTests
{
    private readonly Mock<IRepository<Patient>> _mockRepository;
    private readonly GetAllPatientsQueryHandler _handler;
    private readonly Faker<Patient> _patientFaker;

    public GetAllPatientsQueryHandlerTests()
    {
        _mockRepository = new Mock<IRepository<Patient>>();
        _handler = new GetAllPatientsQueryHandler(_mockRepository.Object);

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
    public async Task Handle_Should_Return_All_Patients_As_DTOs()
    {
        // Arrange
        var patients = _patientFaker.Generate(3);
        var query = new GetAllPatientsQuery();

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(patients);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        var resultList = result.ToList();
        for (int i = 0; i < patients.Count; i++)
        {
            var patient = patients[i];
            var dto = resultList[i];

            dto.Id.Should().Be(patient.Id);
            dto.FirstName.Should().Be(patient.FirstName);
            dto.LastName.Should().Be(patient.LastName);
            dto.Email.Should().Be(patient.Email);
            dto.PhoneNumber.Should().Be(patient.PhoneNumber);
            dto.DateOfBirth.Should().Be(patient.DateOfBirth);
            dto.Gender.Should().Be(patient.Gender);
            dto.Address.Should().Be(patient.Address);
            dto.EmergencyContact.Should().Be(patient.EmergencyContact);
            dto.EmergencyContactPhone.Should().Be(patient.EmergencyContactPhone);
            dto.CreatedAt.Should().Be(patient.CreatedAt);
            dto.UpdatedAt.Should().Be(patient.UpdatedAt);
        }

        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_Collection_When_No_Patients()
    {
        // Arrange
        var query = new GetAllPatientsQuery();

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Patient>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_GetAllAsync()
    {
        // Arrange
        var query = new GetAllPatientsQuery();

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Patient>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Map_All_Patient_Properties_Correctly()
    {
        // Arrange
        var patient = _patientFaker.Generate();
        var query = new GetAllPatientsQuery();

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Patient> { patient });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Single();
        dto.Id.Should().Be(patient.Id);
        dto.FirstName.Should().Be(patient.FirstName);
        dto.LastName.Should().Be(patient.LastName);
        dto.Email.Should().Be(patient.Email);
        dto.PhoneNumber.Should().Be(patient.PhoneNumber);
        dto.DateOfBirth.Should().Be(patient.DateOfBirth);
        dto.Gender.Should().Be(patient.Gender);
        dto.Address.Should().Be(patient.Address);
        dto.EmergencyContact.Should().Be(patient.EmergencyContact);
        dto.EmergencyContactPhone.Should().Be(patient.EmergencyContactPhone);
        dto.CreatedAt.Should().Be(patient.CreatedAt);
        dto.UpdatedAt.Should().Be(patient.UpdatedAt);
    }
}
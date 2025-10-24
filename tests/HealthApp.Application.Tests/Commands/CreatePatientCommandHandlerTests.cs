using FluentAssertions;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;
using HealthApp.Application.Handlers;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using HealthApp.Domain.Interfaces;
using Moq;
using Bogus;

namespace HealthApp.Application.Tests.Commands;

public class CreatePatientCommandHandlerTests
{
    private readonly Mock<IRepository<Patient>> _mockRepository;
    private readonly CreatePatientCommandHandler _handler;
    private readonly Faker<CreatePatientDto> _createPatientDtoFaker;

    public CreatePatientCommandHandlerTests()
    {
        _mockRepository = new Mock<IRepository<Patient>>();
        _handler = new CreatePatientCommandHandler(_mockRepository.Object);

        _createPatientDtoFaker = new Faker<CreatePatientDto>()
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
    public async Task Handle_Should_Create_Patient_Successfully()
    {
        // Arrange
        var createPatientDto = _createPatientDtoFaker.Generate();
        var command = new CreatePatientCommand(createPatientDto);
        var expectedPatient = new Patient();

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync((Patient p) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.FirstName.Should().Be(createPatientDto.FirstName);
        result.LastName.Should().Be(createPatientDto.LastName);
        result.Email.Should().Be(createPatientDto.Email);
        result.PhoneNumber.Should().Be(createPatientDto.PhoneNumber);
        result.DateOfBirth.Should().Be(createPatientDto.DateOfBirth);
        result.Gender.Should().Be(createPatientDto.Gender);
        result.Address.Should().Be(createPatientDto.Address);
        result.EmergencyContact.Should().Be(createPatientDto.EmergencyContact);
        result.EmergencyContactPhone.Should().Be(createPatientDto.EmergencyContactPhone);

        _mockRepository.Verify(x => x.AddAsync(It.Is<Patient>(p =>
            p.FirstName == createPatientDto.FirstName &&
            p.LastName == createPatientDto.LastName &&
            p.Email == createPatientDto.Email &&
            p.PhoneNumber == createPatientDto.PhoneNumber &&
            p.DateOfBirth == createPatientDto.DateOfBirth &&
            p.Gender == createPatientDto.Gender &&
            p.Address == createPatientDto.Address &&
            p.EmergencyContact == createPatientDto.EmergencyContact &&
            p.EmergencyContactPhone == createPatientDto.EmergencyContactPhone
        )), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Set_New_Guid_For_Patient()
    {
        // Arrange
        var createPatientDto = _createPatientDtoFaker.Generate();
        var command = new CreatePatientCommand(createPatientDto);

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync((Patient p) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_AddAsync()
    {
        // Arrange
        var createPatientDto = _createPatientDtoFaker.Generate();
        var command = new CreatePatientCommand(createPatientDto);

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync((Patient p) => p);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Patient>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_Should_Accept_Empty_Optional_Fields(string emptyValue)
    {
        // Arrange
        var createPatientDto = new CreatePatientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123-456-7890",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = Gender.Male,
            Address = emptyValue,
            EmergencyContact = emptyValue,
            EmergencyContactPhone = emptyValue
        };
        var command = new CreatePatientCommand(createPatientDto);

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync((Patient p) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Address.Should().Be(emptyValue);
        result.EmergencyContact.Should().Be(emptyValue);
        result.EmergencyContactPhone.Should().Be(emptyValue);
    }
}
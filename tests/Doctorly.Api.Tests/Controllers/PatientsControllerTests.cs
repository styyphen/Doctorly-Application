using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Doctorly.Api.Controllers;
using HealthApp.Application.Commands;
using HealthApp.Application.Queries;
using HealthApp.Application.DTOs;
using HealthApp.Domain.Enums;
using Bogus;

namespace Doctorly.Api.Tests.Controllers;

public class PatientsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly PatientsController _controller;
    private readonly Faker<PatientDto> _patientDtoFaker;
    private readonly Faker<CreatePatientDto> _createPatientDtoFaker;

    public PatientsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new PatientsController(_mockMediator.Object);

        _patientDtoFaker = new Faker<PatientDto>()
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
    public async Task GetAllPatients_Should_Return_Ok_With_Patients()
    {
        // Arrange
        var patients = _patientDtoFaker.Generate(3);
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllPatientsQuery>(), default))
            .ReturnsAsync(patients);

        // Act
        var result = await _controller.GetAllPatients();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPatients = okResult.Value.Should().BeAssignableTo<IEnumerable<PatientDto>>().Subject;
        returnedPatients.Should().HaveCount(3);
        returnedPatients.Should().BeEquivalentTo(patients);

        _mockMediator.Verify(m => m.Send(It.IsAny<GetAllPatientsQuery>(), default), Times.Once);
    }

    [Fact]
    public async Task GetPatient_Should_Return_Ok_When_Patient_Exists()
    {
        // Arrange
        var patient = _patientDtoFaker.Generate();
        var patientId = patient.Id;

        _mockMediator.Setup(m => m.Send(It.Is<GetPatientByIdQuery>(q => q.PatientId == patientId), default))
            .ReturnsAsync(patient);

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPatient = okResult.Value.Should().BeOfType<PatientDto>().Subject;
        returnedPatient.Should().BeEquivalentTo(patient);

        _mockMediator.Verify(m => m.Send(It.Is<GetPatientByIdQuery>(q => q.PatientId == patientId), default), Times.Once);
    }

    [Fact]
    public async Task GetPatient_Should_Return_NotFound_When_Patient_Not_Exists()
    {
        // Arrange
        var patientId = Guid.NewGuid();

        _mockMediator.Setup(m => m.Send(It.Is<GetPatientByIdQuery>(q => q.PatientId == patientId), default))
            .ReturnsAsync((PatientDto?)null);

        // Act
        var result = await _controller.GetPatient(patientId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        _mockMediator.Verify(m => m.Send(It.Is<GetPatientByIdQuery>(q => q.PatientId == patientId), default), Times.Once);
    }

    [Fact]
    public async Task CreatePatient_Should_Return_Created_When_Valid()
    {
        // Arrange
        var createPatientDto = _createPatientDtoFaker.Generate();
        var createdPatient = _patientDtoFaker.Generate();

        _mockMediator.Setup(m => m.Send(It.IsAny<CreatePatientCommand>(), default))
            .ReturnsAsync(createdPatient);

        // Act
        var result = await _controller.CreatePatient(createPatientDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(PatientsController.GetPatient));
        createdResult.RouteValues!["id"].Should().Be(createdPatient.Id);

        var returnedPatient = createdResult.Value.Should().BeOfType<PatientDto>().Subject;
        returnedPatient.Should().BeEquivalentTo(createdPatient);

        _mockMediator.Verify(m => m.Send(It.IsAny<CreatePatientCommand>(), default), Times.Once);
    }

    [Fact]
    public async Task CreatePatient_Should_Return_BadRequest_When_ModelState_Invalid()
    {
        // Arrange
        var createPatientDto = _createPatientDtoFaker.Generate();
        _controller.ModelState.AddModelError("FirstName", "FirstName is required");

        // Act
        var result = await _controller.CreatePatient(createPatientDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();

        _mockMediator.Verify(m => m.Send(It.IsAny<CreatePatientCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task CreatePatient_Should_Send_Correct_Command()
    {
        // Arrange
        var createPatientDto = _createPatientDtoFaker.Generate();
        var createdPatient = _patientDtoFaker.Generate();

        _mockMediator.Setup(m => m.Send(It.IsAny<CreatePatientCommand>(), default))
            .ReturnsAsync(createdPatient);

        // Act
        await _controller.CreatePatient(createPatientDto);

        // Assert
        _mockMediator.Verify(m => m.Send(
            It.Is<CreatePatientCommand>(cmd =>
                cmd.PatientDto.FirstName == createPatientDto.FirstName &&
                cmd.PatientDto.LastName == createPatientDto.LastName &&
                cmd.PatientDto.Email == createPatientDto.Email &&
                cmd.PatientDto.PhoneNumber == createPatientDto.PhoneNumber &&
                cmd.PatientDto.DateOfBirth == createPatientDto.DateOfBirth &&
                cmd.PatientDto.Gender == createPatientDto.Gender &&
                cmd.PatientDto.Address == createPatientDto.Address &&
                cmd.PatientDto.EmergencyContact == createPatientDto.EmergencyContact &&
                cmd.PatientDto.EmergencyContactPhone == createPatientDto.EmergencyContactPhone
            ),
            default), Times.Once);
    }

    [Fact]
    public async Task GetAllPatients_Should_Return_Empty_Collection_When_No_Patients()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllPatientsQuery>(), default))
            .ReturnsAsync(new List<PatientDto>());

        // Act
        var result = await _controller.GetAllPatients();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPatients = okResult.Value.Should().BeAssignableTo<IEnumerable<PatientDto>>().Subject;
        returnedPatients.Should().BeEmpty();

        _mockMediator.Verify(m => m.Send(It.IsAny<GetAllPatientsQuery>(), default), Times.Once);
    }
}
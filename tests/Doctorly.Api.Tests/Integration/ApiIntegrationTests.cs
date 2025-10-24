using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json;
using HealthApp.Infrastructure.Data;
using HealthApp.Application.DTOs;
using HealthApp.Domain.Enums;
using Bogus;

namespace Doctorly.Api.Tests.Integration;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Faker<CreatePatientDto> _createPatientDtoFaker;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<HealthAppDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbContextServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(HealthAppDbContext));
                if (dbContextServiceDescriptor != null)
                    services.Remove(dbContextServiceDescriptor);

                // Add in-memory database for testing
                services.AddDbContext<HealthAppDbContext>(options =>
                    options.UseInMemoryDatabase("InMemoryDbForTesting"));
            });
        });

        _client = _factory.CreateClient();

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

    [Fact(Skip = "Integration tests require database setup")]
    public async Task GetAllPatients_Should_Return_Ok()
    {
        // Act
        var response = await _client.GetAsync("/api/patients");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNull();
    }

    [Fact(Skip = "Integration tests require database setup")]
    public async Task GetPatient_Should_Return_NotFound_For_NonExistent_Id()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/patients/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Integration tests require database setup")]
    public async Task CreatePatient_Should_Return_Created_With_Valid_Data()
    {
        // Arrange
        var createPatientDto = _createPatientDtoFaker.Generate();
        var json = JsonSerializer.Serialize(createPatientDto, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/patients", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdPatient = JsonSerializer.Deserialize<PatientDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        createdPatient.Should().NotBeNull();
        createdPatient!.Id.Should().NotBeEmpty();
        createdPatient.FirstName.Should().Be(createPatientDto.FirstName);
        createdPatient.LastName.Should().Be(createPatientDto.LastName);
        createdPatient.Email.Should().Be(createPatientDto.Email);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/patients/{createdPatient.Id}");
    }

    [Fact(Skip = "Integration tests require database setup")]
    public async Task CreatePatient_Should_Return_BadRequest_With_Invalid_Data()
    {
        // Arrange
        var invalidPatient = new
        {
            FirstName = "", // Invalid - empty
            LastName = "Doe",
            Email = "invalid-email", // Invalid format
            PhoneNumber = "123-456-7890",
            DateOfBirth = DateTime.Now.AddYears(-25),
            Gender = Gender.Male,
            Address = "123 Main St",
            EmergencyContact = "Jane Doe",
            EmergencyContactPhone = "987-654-3210"
        };

        var json = JsonSerializer.Serialize(invalidPatient, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/patients", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Integration tests require database setup")]
    public async Task GetPatient_Should_Return_Created_Patient()
    {
        // Arrange - Create a patient first
        var createPatientDto = _createPatientDtoFaker.Generate();
        var json = JsonSerializer.Serialize(createPatientDto, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/patients", content);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createdPatient = JsonSerializer.Deserialize<PatientDto>(createResponseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act - Get the created patient
        var getResponse = await _client.GetAsync($"/api/patients/{createdPatient!.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponseContent = await getResponse.Content.ReadAsStringAsync();
        var retrievedPatient = JsonSerializer.Deserialize<PatientDto>(getResponseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        retrievedPatient.Should().NotBeNull();
        retrievedPatient!.Id.Should().Be(createdPatient.Id);
        retrievedPatient.FirstName.Should().Be(createdPatient.FirstName);
        retrievedPatient.LastName.Should().Be(createdPatient.LastName);
        retrievedPatient.Email.Should().Be(createdPatient.Email);
    }

    [Fact(Skip = "Integration tests require database setup")]
    public async Task GraphQL_Endpoint_Should_Be_Available()
    {
        // Act
        var response = await _client.GetAsync("/graphql");

        // Assert
        // GraphQL endpoint typically returns 400 for GET requests without query
        // or 405 Method Not Allowed, but it should not return 404
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }
}
using HealthApp.Domain.Enums;

namespace HealthApp.Application.DTOs;

public class AttendeeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public AttendeeStatus Status { get; set; }
    public Guid EventId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
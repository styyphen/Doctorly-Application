using System.ComponentModel.DataAnnotations;
using HealthApp.Domain.Enums;

namespace HealthApp.Domain.Entities;

public class Attendee
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string EmailAddress { get; set; } = string.Empty;

    public AttendeeStatus Status { get; set; } = AttendeeStatus.Pending;

    public Guid EventId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Event Event { get; set; } = null!;
}
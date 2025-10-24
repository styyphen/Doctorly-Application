using System.ComponentModel.DataAnnotations;

namespace HealthApp.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();
}
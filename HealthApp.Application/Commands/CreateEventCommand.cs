using MediatR;
using HealthApp.Application.DTOs;

namespace HealthApp.Application.Commands;

public class CreateEventCommand : IRequest<EventDto>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<CreateAttendeeRequest> Attendees { get; set; } = new();
}

public class CreateAttendeeRequest
{
    public string Name { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
}
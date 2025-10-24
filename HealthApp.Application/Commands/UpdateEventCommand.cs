using MediatR;
using HealthApp.Application.DTOs;

namespace HealthApp.Application.Commands;

public class UpdateEventCommand : IRequest<EventDto?>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
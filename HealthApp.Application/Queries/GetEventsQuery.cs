using MediatR;
using HealthApp.Application.DTOs;

namespace HealthApp.Application.Queries;

public class GetEventsQuery : IRequest<IEnumerable<EventDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
}
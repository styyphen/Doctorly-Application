using MediatR;
using HealthApp.Application.DTOs;

namespace HealthApp.Application.Queries;

public class GetEventByIdQuery : IRequest<EventDto?>
{
    public Guid Id { get; set; }
}
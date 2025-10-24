using MediatR;
using HealthApp.Application.Queries;
using HealthApp.Application.DTOs;
using HealthApp.Domain.Interfaces;

namespace HealthApp.Application.Handlers;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto?>
{
    private readonly IEventRepository _eventRepository;

    public GetEventByIdQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventDto?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventWithAttendeesAsync(request.Id);

        if (eventEntity == null)
            return null;

        return new EventDto
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            StartTime = eventEntity.StartTime,
            EndTime = eventEntity.EndTime,
            CreatedAt = eventEntity.CreatedAt,
            UpdatedAt = eventEntity.UpdatedAt,
            Attendees = eventEntity.Attendees.Select(a => new AttendeeDto
            {
                Id = a.Id,
                Name = a.Name,
                EmailAddress = a.EmailAddress,
                Status = a.Status,
                EventId = a.EventId,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList()
        };
    }
}
using MediatR;
using HealthApp.Application.Queries;
using HealthApp.Application.DTOs;
using HealthApp.Domain.Interfaces;

namespace HealthApp.Application.Handlers;

public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, IEnumerable<EventDto>>
{
    private readonly IEventRepository _eventRepository;

    public GetEventsQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Event> events;

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            events = await _eventRepository.SearchEventsAsync(request.SearchTerm);
        }
        else if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            events = await _eventRepository.GetEventsByDateRangeAsync(request.StartDate.Value, request.EndDate.Value);
        }
        else
        {
            events = await _eventRepository.GetAllAsync();
        }

        return events.Select(e => new EventDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            Attendees = e.Attendees.Select(a => new AttendeeDto
            {
                Id = a.Id,
                Name = a.Name,
                EmailAddress = a.EmailAddress,
                Status = a.Status,
                EventId = a.EventId,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList()
        });
    }
}
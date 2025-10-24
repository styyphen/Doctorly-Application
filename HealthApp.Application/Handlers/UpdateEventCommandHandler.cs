using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;
using HealthApp.Application.Services;
using HealthApp.Domain.Interfaces;

namespace HealthApp.Application.Handlers;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, EventDto?>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEmailService _emailService;

    public UpdateEventCommandHandler(IEventRepository eventRepository, IEmailService emailService)
    {
        _eventRepository = eventRepository;
        _emailService = emailService;
    }

    public async Task<EventDto?> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(request.Id);

        if (eventEntity == null)
            return null;

        eventEntity.Title = request.Title;
        eventEntity.Description = request.Description;
        eventEntity.StartTime = request.StartTime;
        eventEntity.EndTime = request.EndTime;

        await _eventRepository.UpdateAsync(eventEntity);

        // Send update notifications to attendees
        foreach (var attendee in eventEntity.Attendees)
        {
            await _emailService.SendEventUpdatedNotificationAsync(
                attendee.EmailAddress,
                attendee.Name,
                eventEntity.Title,
                eventEntity.StartTime,
                eventEntity.EndTime);
        }

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
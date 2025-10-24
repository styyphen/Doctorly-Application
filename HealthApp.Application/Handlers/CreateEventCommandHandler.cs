using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;
using HealthApp.Application.Services;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Interfaces;
using HealthApp.Domain.Enums;

namespace HealthApp.Application.Handlers;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEmailService _emailService;

    public CreateEventCommandHandler(IEventRepository eventRepository, IEmailService emailService)
    {
        _eventRepository = eventRepository;
        _emailService = emailService;
    }

    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Attendees = request.Attendees.Select(a => new Attendee
            {
                Id = Guid.NewGuid(),
                Name = a.Name,
                EmailAddress = a.EmailAddress,
                Status = AttendeeStatus.Pending
            }).ToList()
        };

        await _eventRepository.AddAsync(eventEntity);

        // Send invitations to attendees
        foreach (var attendee in eventEntity.Attendees)
        {
            await _emailService.SendAttendeeInvitationAsync(
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
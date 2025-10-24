using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Application.DTOs;
using HealthApp.Application.Services;
using HealthApp.Domain.Interfaces;

namespace HealthApp.Application.Handlers;

public class UpdateAttendeeStatusCommandHandler : IRequestHandler<UpdateAttendeeStatusCommand, AttendeeDto?>
{
    private readonly IAttendeeRepository _attendeeRepository;
    private readonly IEmailService _emailService;
    private readonly IEventRepository _eventRepository;

    public UpdateAttendeeStatusCommandHandler(IAttendeeRepository attendeeRepository, IEmailService emailService, IEventRepository eventRepository)
    {
        _attendeeRepository = attendeeRepository;
        _emailService = emailService;
        _eventRepository = eventRepository;
    }

    public async Task<AttendeeDto?> Handle(UpdateAttendeeStatusCommand request, CancellationToken cancellationToken)
    {
        var attendee = await _attendeeRepository.GetByIdAsync(request.AttendeeId);

        if (attendee == null)
            return null;

        attendee.Status = request.Status;
        await _attendeeRepository.UpdateAsync(attendee);

        // Load the event to get organizer information (for demonstration, we'll use a placeholder)
        var eventEntity = await _eventRepository.GetByIdAsync(attendee.EventId);
        if (eventEntity != null)
        {
            // Note: In a real system, you'd have an organizer field or derive it from the event creator
            var organizerEmail = "organizer@healthapp.com"; // Placeholder - would be actual organizer email
            await _emailService.SendAttendeeStatusChangedAsync(
                organizerEmail,
                attendee.Name,
                eventEntity.Title,
                attendee.Status.ToString());
        }

        return new AttendeeDto
        {
            Id = attendee.Id,
            Name = attendee.Name,
            EmailAddress = attendee.EmailAddress,
            Status = attendee.Status,
            EventId = attendee.EventId,
            CreatedAt = attendee.CreatedAt,
            UpdatedAt = attendee.UpdatedAt
        };
    }
}
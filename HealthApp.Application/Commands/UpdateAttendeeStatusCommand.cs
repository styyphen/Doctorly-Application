using MediatR;
using HealthApp.Application.DTOs;
using HealthApp.Domain.Enums;

namespace HealthApp.Application.Commands;

public class UpdateAttendeeStatusCommand : IRequest<AttendeeDto?>
{
    public Guid AttendeeId { get; set; }
    public AttendeeStatus Status { get; set; }
}
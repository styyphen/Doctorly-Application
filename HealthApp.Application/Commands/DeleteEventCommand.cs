using MediatR;

namespace HealthApp.Application.Commands;

public class DeleteEventCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
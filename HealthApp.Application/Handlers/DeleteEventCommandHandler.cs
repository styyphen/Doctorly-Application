using MediatR;
using HealthApp.Application.Commands;
using HealthApp.Domain.Interfaces;

namespace HealthApp.Application.Handlers;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, bool>
{
    private readonly IEventRepository _eventRepository;

    public DeleteEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(request.Id);

        if (eventEntity == null)
            return false;

        await _eventRepository.DeleteAsync(eventEntity);
        return true;
    }
}
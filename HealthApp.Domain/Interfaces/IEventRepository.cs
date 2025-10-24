using HealthApp.Domain.Entities;

namespace HealthApp.Domain.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
    Task<Event?> GetEventWithAttendeesAsync(Guid eventId);
}
using HealthApp.Domain.Entities;

namespace HealthApp.Domain.Interfaces;

public interface IAttendeeRepository : IRepository<Attendee>
{
    Task<IEnumerable<Attendee>> GetAttendeesByEventAsync(Guid eventId);
    Task<Attendee?> GetAttendeeByEmailAndEventAsync(string email, Guid eventId);
}
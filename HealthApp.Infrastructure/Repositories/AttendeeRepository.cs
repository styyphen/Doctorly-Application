using Microsoft.EntityFrameworkCore;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Interfaces;
using HealthApp.Infrastructure.Data;

namespace HealthApp.Infrastructure.Repositories;

public class AttendeeRepository : Repository<Attendee>, IAttendeeRepository
{
    public AttendeeRepository(HealthAppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Attendee>> GetAttendeesByEventAsync(Guid eventId)
    {
        return await _context.Attendees
            .Include(a => a.Event)
            .Where(a => a.EventId == eventId)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Attendee?> GetAttendeeByEmailAndEventAsync(string email, Guid eventId)
    {
        return await _context.Attendees
            .Include(a => a.Event)
            .FirstOrDefaultAsync(a => a.EmailAddress == email && a.EventId == eventId);
    }
}
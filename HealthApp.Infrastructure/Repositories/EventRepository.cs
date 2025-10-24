using Microsoft.EntityFrameworkCore;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Interfaces;
using HealthApp.Infrastructure.Data;

namespace HealthApp.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(HealthAppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Events
            .Include(e => e.Attendees)
            .Where(e => e.StartTime >= startDate && e.StartTime <= endDate)
            .OrderBy(e => e.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
    {
        return await _context.Events
            .Include(e => e.Attendees)
            .Where(e => e.Title.Contains(searchTerm) || e.Description.Contains(searchTerm))
            .OrderBy(e => e.StartTime)
            .ToListAsync();
    }

    public async Task<Event?> GetEventWithAttendeesAsync(Guid eventId)
    {
        return await _context.Events
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }
}
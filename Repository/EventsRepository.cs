using EventBookingAPI.Data;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBookingAPI.Repository;

public class EventsRepository : IEventsRepository
{
    private readonly AppDbContext _dbContext;
    public EventsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<(List<Event> Items, int TotalCount)> GetEventsPageAsync(int pageNumber, int pageSize)
    {
        var find = _dbContext.Events.AsNoTracking();
         
        var count = await find.CountAsync();
        
        var items = await find
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
            .ToListAsync();
        return (items, count);
    }

    public async Task<Event?> GetEventsByIdAsync(int eventId)
    {
        var find = await  _dbContext.Events.FindAsync(eventId);
        return find;
    }
    public async Task AddEventAsync(Event ev)
    {
        await _dbContext.Events.AddAsync(ev);
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _dbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public void DeleteEvent(Event ev)
    {
        _dbContext.Events.Remove(ev);
    }
    
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

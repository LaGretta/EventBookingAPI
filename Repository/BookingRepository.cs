using EventBookingAPI.Data;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBookingAPI.Repository;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbContext;

    public BookingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<Booking> Items, int TotalCount)> GetEventsPageAsync(int pageNumber, int pageSize, int userId)
    {
        var find = _dbContext.Bookings
            .Include(n => n.Event)
            .Where(n => n.UserId == userId && n.EventId > 0)
            .AsNoTracking();
        
        var count = await find.CountAsync();
        
        var items = await find
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, count);
    }

    public async Task CreateBookingAsync(Booking booking)
    {
        await _dbContext.Bookings.AddAsync(booking);
    }

    public async Task<Booking?> GetBookingByIdAsync(int id, int userId)
    {
         var find = await  _dbContext.Bookings
             .Include(n => n.Event)
             .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
         
         return find;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

}

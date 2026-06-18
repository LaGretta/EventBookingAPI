using EventBookingAPI.Models;

namespace EventBookingAPI.Interfaces.Repository;

public interface IBookingRepository
{
    public Task<(List<Booking> Items, int TotalCount)> GetEventsPageAsync(int pageNumber, int pageSize , int userId);
    Task CreateBookingAsync(Booking booking);
    Task<Booking?> GetBookingByIdAsync(int id , int userId);
    Task SaveChangesAsync();

}
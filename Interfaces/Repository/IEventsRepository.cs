using EventBookingAPI.Models;

namespace EventBookingAPI.Interfaces.Repository;

public interface IEventsRepository
{
    public Task<(List<Event> Items, int TotalCount)> GetEventsPageAsync(int pageNumber, int pageSize);
    Task<Event?> GetEventsByIdAsync(int eventId);
    Task AddEventAsync(Event ev);
    Task<Event?> GetEventByIdAsync(int id);
    public void DeleteEvent(Event ev);
    Task SaveChangesAsync();
}

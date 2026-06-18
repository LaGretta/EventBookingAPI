namespace EventBookingAPI.Models;

public class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public decimal Price { get; set; }

    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<Booking> Bookings { get; set; } = new();
}
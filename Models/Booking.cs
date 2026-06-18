namespace EventBookingAPI.Models;

public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int SeatsCount { get; set; }
    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
}
public enum UserRole
{
    User = 1,
    Admin = 2
}

public enum EventStatus
{
    Draft = 1,
    Published = 2,
    Cancelled = 3,
    Completed = 4
}

public enum BookingStatus
{
    Active = 1,
    Cancelled = 2
}
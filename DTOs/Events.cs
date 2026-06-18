using EventBookingAPI.Models;

namespace EventBookingAPI.DTOs;

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    
    public EventStatus Status { get; set; }
    public DateTime EventDate { get; set; }

    public decimal Price { get; set; }
}
public class UpdateEventDto
{
    
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public decimal Price { get; set; }

    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }

    public EventStatus Status { get; set; } 

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
public class EventResponseDto
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
}
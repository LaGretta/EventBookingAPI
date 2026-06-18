using AutoMapper;
using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Interfaces.Service;
using EventBookingAPI.Models;

namespace EventBookingAPI.Service;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly IMapper _mapper;

    public BookingService(IBookingRepository bookingRepository, IEventsRepository eventsRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _eventsRepository = eventsRepository;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> CreateAsync(CreateBookingDto dto, int userId)
    {
        var ev = await _eventsRepository.GetEventByIdAsync(dto.EventId);
        if (ev == null)
            throw new Exception("Event not found");

        if (ev.Status != EventStatus.Published)
            throw new Exception("Only published events can be booked");

        if (dto.SeatsCount > ev.AvailableSeats)
            throw new Exception("Not enough seats available");

        var booking = _mapper.Map<Booking>(dto);
        booking.UserId = userId;
        booking.EventId = ev.Id;
        booking.Status = BookingStatus.Active;
        booking.TotalPrice = ev.Price * dto.SeatsCount;
        booking.CreatedAt = DateTime.UtcNow;

        ev.AvailableSeats -= dto.SeatsCount;

        await _bookingRepository.CreateBookingAsync(booking);
        await _bookingRepository.SaveChangesAsync();

        return _mapper.Map<BookingResponseDto>(booking);
    }

    public async Task<PagedResponseDto<BookingResponseDto>> GetPagedAsync(int pageNumber, int pageSize, int userId)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = await _bookingRepository.GetEventsPageAsync(pageNumber, pageSize, userId);
        var map = _mapper.Map<List<BookingResponseDto>>(paged.Items);

        return new PagedResponseDto<BookingResponseDto>
        {
            Items = map,
            Page = pageNumber,
            PageSize = pageSize,
            TotalCount = paged.TotalCount,
            TotalPages = (int)Math.Ceiling((double)paged.TotalCount / pageSize)
        };
    }

    public async Task<BookingResponseDto> GetByIdAsync(int id, int userId)
    {
        var find = await _bookingRepository.GetBookingByIdAsync(id, userId);
        if (find == null)
            throw new Exception("Booking not found");

        return _mapper.Map<BookingResponseDto>(find);
    }

    public async Task<BookingResponseDto?> CancelBookingAsync(int id, int userId)
    {
        var booking = await _bookingRepository.GetBookingByIdAsync(id, userId);

        if (booking == null)
            return null;

        if (booking.Status == BookingStatus.Cancelled)
            throw new Exception("Booking is already cancelled.");

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        booking.Event.AvailableSeats += booking.SeatsCount;

        await _bookingRepository.SaveChangesAsync();

        return _mapper.Map<BookingResponseDto>(booking);
    }
}

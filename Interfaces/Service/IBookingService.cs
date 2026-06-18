using EventBookingAPI.DTOs;

namespace EventBookingAPI.Interfaces.Service;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(CreateBookingDto dto, int userId);
    Task<PagedResponseDto<BookingResponseDto>> GetPagedAsync(int pageNumber, int pageSize , int userId);
    Task<BookingResponseDto> GetByIdAsync(int id , int userId);
    Task<BookingResponseDto?> CancelBookingAsync(int id, int userId);

}

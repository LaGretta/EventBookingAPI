using EventBookingAPI.DTOs;

namespace EventBookingAPI.Interfaces.Service;

public interface IEventsService
{
    Task<PagedResponseDto<EventResponseDto>> GetPagedAsync(int pageNumber, int pageSize);
    Task<EventResponseDto> GetByIdAsync(int id);
    Task<EventResponseDto> CreateAsync(CreateEventDto createEventDto);
    Task<EventResponseDto> UpdateAsync(int id, UpdateEventDto updateEventDto);
    Task DeleteAsync(int id);
    Task<EventResponseDto> UpdatePublishAsync(int id);
    Task<EventResponseDto>  CancelEventAsync(int id);
    
}

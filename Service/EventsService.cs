using AutoMapper;
using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Interfaces.Service;
using EventBookingAPI.Models;

namespace EventBookingAPI.Service;

public class EventsService : IEventsService
{
    private readonly IEventsRepository _repository;
    private readonly IMapper _mapper;

    public EventsService(IEventsRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResponseDto<EventResponseDto>> GetPagedAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var paged = await _repository.GetEventsPageAsync(pageNumber, pageSize);
        var items = _mapper.Map<List<EventResponseDto>>(paged.Items);

        return new PagedResponseDto<EventResponseDto>
        {
            Items = items,
            Page = pageNumber,
            PageSize = pageSize,
            TotalCount = paged.TotalCount,
            TotalPages = (int)Math.Ceiling((double)paged.TotalCount / pageSize)
        };
    }

    public async Task<EventResponseDto> GetByIdAsync(int id)
    {
        var ev = await _repository.GetEventByIdAsync(id);

        if (ev == null)
            throw new Exception("Event not found");

        return _mapper.Map<EventResponseDto>(ev);
    }

    public async Task<EventResponseDto> CreateAsync(CreateEventDto createEventDto)
    {
        var ev = _mapper.Map<Event>(createEventDto);
        ev.CreatedAt = DateTime.UtcNow;
        ev.UpdatedAt = DateTime.UtcNow;

        await _repository.AddEventAsync(ev);
        await _repository.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(ev);
    }

    public async Task<EventResponseDto> UpdateAsync(int id, UpdateEventDto updateEventDto)
    {
        var ev = await _repository.GetEventByIdAsync(id);
        if (ev == null)
            throw new Exception("Event not found");

        _mapper.Map(updateEventDto, ev);
        ev.Id = id;
        ev.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(ev);
    }

    public async Task DeleteAsync(int id)
    {
        var ev = await _repository.GetEventByIdAsync(id);
        if (ev == null)
            throw new Exception("Event not found");

        _repository.DeleteEvent(ev);
        await _repository.SaveChangesAsync();
    }

    public async Task<EventResponseDto> UpdatePublishAsync(int id)
    {
        var ev = await _repository.GetEventByIdAsync(id);
        if (ev == null)
            throw new Exception("Event not found");

        ev.Status = EventStatus.Published;
        ev.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(ev);
    }

    public async Task<EventResponseDto> CancelEventAsync(int id)
    {
        var ev = await _repository.GetEventByIdAsync(id);
        if (ev == null)
            throw new Exception("Event not found");

        ev.Status = EventStatus.Cancelled;
        ev.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(ev);
    }
}

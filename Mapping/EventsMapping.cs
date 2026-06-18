using AutoMapper;
using EventBookingAPI.DTOs;
using EventBookingAPI.Models;

namespace EventBookingAPI.Mapping;

public class EventsMapping : Profile
{
    public  EventsMapping()
    {
        CreateMap<CreateEventDto, Event>();
        CreateMap<UpdateEventDto, Event>();
        CreateMap<Event, EventResponseDto>();
    }
}
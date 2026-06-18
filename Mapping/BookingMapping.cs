using AutoMapper;
using EventBookingAPI.DTOs;
using EventBookingAPI.Models;

namespace EventBookingAPI.Mapping;

public class BookingMapping : Profile
{
    public BookingMapping()
    {
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<Booking, BookingResponseDto>();
    }
}
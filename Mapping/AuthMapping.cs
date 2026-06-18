using AutoMapper;
using EventBookingAPI.DTOs;
using EventBookingAPI.Models;

namespace EventBookingAPI.Mapping;

public class AuthMapping : Profile
{
   public AuthMapping()
   {
      CreateMap<RegisterDto, User>();
      CreateMap<User, AuthResponseDto>();
   }
}
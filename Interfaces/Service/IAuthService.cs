using EventBookingAPI.DTOs;

namespace EventBookingAPI.Interfaces.Service;

public interface IAuthService
{
    Task<AuthResponseDto> Register(RegisterDto dto);
    Task<AuthResponseDto> Login(LoginDto dto);
}
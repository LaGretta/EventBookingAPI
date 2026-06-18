using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Interfaces.Service;
using EventBookingAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace EventBookingAPI.Service;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;

    public AuthService(IAuthRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task<AuthResponseDto> Register(RegisterDto dto)
    {
        var exists = await _repo.ExistsUserByEmailAsync(dto.Email);
        if (exists)
            throw new Exception("Email already exists");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        await _repo.AddUserAsync(user);
        await _repo.SaveChangesAsync();

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
        var user = await _repo.GetUserByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("Email or password is incorrect");

        var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isValid)
            throw new Exception("Email or password is incorrect");

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        return new AuthResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = GenerateJwtToken(user),
            CreatedAt = user.CreatedAt
        };
    }

    private string GenerateJwtToken(User user)
    {
        var sec = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var cred = new SigningCredentials(sec, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken
        (
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.Now.AddDays(10),
            claims: claims,
            signingCredentials: cred
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

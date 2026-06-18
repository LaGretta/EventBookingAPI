using EventBookingAPI.Data;
using EventBookingAPI.Interfaces.Repository;
using EventBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBookingAPI.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var find = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return find;
    }

    public async Task<bool> ExistsUserByEmailAsync(string email)
    {
        var find = await _context.Users.AnyAsync(x => x.Email == email);
        return find;
    }
    public async Task AddUserAsync(User user)
    {
      await _context.Users.AddAsync(user);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

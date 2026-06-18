using EventBookingAPI.Models;

namespace EventBookingAPI.Interfaces.Repository;

public interface IAuthRepository
{
    Task<User?>  GetUserByEmailAsync(string email);
    Task<bool> ExistsUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();

}
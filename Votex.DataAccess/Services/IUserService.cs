using Votex.DataAccess.Models;

namespace Votex.DataAccess.Services
{
    public interface IUserService
    {
        Task AddUserAsync(User user, string password);
        Task<(string authToken, string refreshToken, int userId)> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<(string authToken, string refreshToken, int userId)> RedeemRefreshTokenAsync(string refreshToken);
        Task<List<User>> GetUsersAsync();
    }
}
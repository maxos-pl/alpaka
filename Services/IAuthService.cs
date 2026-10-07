using System.Threading.Tasks;
using StavZooApp.DTOs;
using StavZooApp.Models;

namespace StavZooApp.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
    }
}

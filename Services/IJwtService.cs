using System.Security.Claims;
using StavZooApp.Models;

namespace StavZooApp.Services
{
    public interface IJwtService
    {
        (string token, DateTime expiration) GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}

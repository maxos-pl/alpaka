using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StavZooApp.Models;

namespace StavZooApp.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public (string token, DateTime expiration) GenerateToken(User user)
        {
            var secretKey = _config["Jwt:Secret"] ?? "StavropolZooSecretSuperSecurityKey2026!@#$%^";
            var issuer = _config["Jwt:Issuer"] ?? "StavZooApp";
            var audience = _config["Jwt:Audience"] ?? "StavZooClients";
            var expiryHours = int.TryParse(_config["Jwt:ExpiryHours"], out var hours) ? hours : 24;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(expiryHours);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Position", user.Position ?? string.Empty),
                new Claim("Username", user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var secretKey = _config["Jwt:Secret"] ?? "StavropolZooSecretSuperSecurityKey2026!@#$%^";
            var issuer = _config["Jwt:Issuer"] ?? "StavZooApp";
            var audience = _config["Jwt:Audience"] ?? "StavZooClients";

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}

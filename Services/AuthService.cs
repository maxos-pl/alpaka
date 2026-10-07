using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StavZooApp.Data;
using StavZooApp.DTOs;
using StavZooApp.Models;

namespace StavZooApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly ZooDbContext _db;
        private readonly IJwtService _jwtService;

        public AuthService(ZooDbContext db, IJwtService jwtService)
        {
            _db = db;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "Пользователь с таким Email не найден" };
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return new AuthResponseDto { Success = false, Message = "Неверный пароль" };
            }

            var (token, expiration) = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Успешная авторизация",
                Token = token,
                Expiration = expiration,
                User = new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Position = user.Position
                }
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
            {
                return new AuthResponseDto { Success = false, Message = "Пользователь с таким Email уже зарегистрирован" };
            }

            if (await _db.Users.AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower()))
            {
                return new AuthResponseDto { Success = false, Message = "Пользователь с таким логином уже существует" };
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role,
                Position = string.IsNullOrWhiteSpace(dto.Position) ? "Посетитель" : dto.Position,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var (token, expiration) = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Регистрация успешна",
                Token = token,
                Expiration = expiration,
                User = new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Position = user.Position
                }
            };
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Position = user.Position
            };
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}

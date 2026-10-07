using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StavZooApp.DTOs;
using StavZooApp.Services;

namespace StavZooApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _authService.LoginAsync(dto);
            if (!result.Success || result.User == null)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }

            await SignInUserAsync(result.User, result.Token);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Animal", "Zoo", new { slug = "alpaca" });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _authService.RegisterAsync(dto);
            if (!result.Success || result.User == null)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }

            await SignInUserAsync(result.User, result.Token);
            return RedirectToAction("Animal", "Zoo", new { slug = "alpaca" });
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("ZooAuthToken");
            return RedirectToAction("Animal", "Zoo", new { slug = "alpaca" });
        }

        // Quick demo login helper for convenience
        [HttpPost]
        public async Task<IActionResult> DemoLogin(string role)
        {
            LoginDto loginDto;
            if (role == "employee")
            {
                loginDto = new LoginDto { Email = "keeper@stavzoo.ru", Password = "Password123!" };
            }
            else if (role == "vet")
            {
                loginDto = new LoginDto { Email = "vet@stavzoo.ru", Password = "Password123!" };
            }
            else
            {
                loginDto = new LoginDto { Email = "guest@mail.ru", Password = "Password123!" };
            }

            var result = await _authService.LoginAsync(loginDto);
            if (result.Success && result.User != null)
            {
                await SignInUserAsync(result.User, result.Token);
            }

            return RedirectToAction("Animal", "Zoo", new { slug = "alpaca" });
        }

        private async Task SignInUserAsync(UserProfileDto user, string? token)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Position", user.Position),
                new Claim("Username", user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });

            if (!string.IsNullOrEmpty(token))
            {
                Response.Cookies.Append("ZooAuthToken", token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false, // Accessible to client-side JS for fetch requests
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }
        }
    }
}

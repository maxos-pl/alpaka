using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StavZooApp.Data;
using StavZooApp.DTOs;
using StavZooApp.Models;

namespace StavZooApp.Controllers.Api
{
    [ApiController]
    [Route("api/donations")]
    [Route("stav-zoo/api/donations")]
    [Produces("application/json")]
    public class DonationsApiController : ControllerBase
    {
        private readonly ZooDbContext _db;

        public DonationsApiController(ZooDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Получение сводки и последних донатов по животному
        /// </summary>
        [HttpGet("{slug}")]
        public async Task<ActionResult<object>> GetDonations(string slug)
        {
            var animal = await _db.Animals
                .Include(a => a.Donations)
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null)
            {
                return NotFound(new { message = $"Животное '{slug}' не найдено" });
            }

            var totalRaised = animal.Donations.Sum(d => d.Amount);
            var goal = animal.MonthlyDonationGoal > 0 ? animal.MonthlyDonationGoal : 50000;
            var percentage = Math.Min(100, (int)Math.Round((totalRaised / goal) * 100));

            var recentDonations = animal.Donations
                .OrderByDescending(d => d.CreatedAt)
                .Take(20)
                .Select(d => new DonationResponseDto
                {
                    Id = d.Id,
                    DonorName = d.DonorName,
                    Amount = d.Amount,
                    Target = d.Target,
                    Message = d.Message,
                    CreatedAt = d.CreatedAt
                })
                .ToList();

            return Ok(new
            {
                summary = new DonationSummaryDto
                {
                    TotalRaised = totalRaised,
                    Goal = goal,
                    Percentage = percentage,
                    DonorsCount = animal.Donations.Count
                },
                donations = recentDonations
            });
        }

        /// <summary>
        /// Отправка благотворительного пожертвования (free donation)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MakeDonation([FromBody] DonationCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == dto.AnimalSlug.ToLower());
            if (animal == null)
            {
                return NotFound(new { message = $"Животное '{dto.AnimalSlug}' не найдено" });
            }

            var donation = new Donation
            {
                AnimalId = animal.Id,
                DonorName = string.IsNullOrWhiteSpace(dto.DonorName) ? "Анонимный друг зоопарка" : dto.DonorName.Trim(),
                Amount = dto.Amount,
                Target = string.IsNullOrWhiteSpace(dto.Target) ? "Корм и лакомства" : dto.Target,
                Message = dto.Message?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            var donationsList = await _db.Donations.Where(d => d.AnimalId == animal.Id).ToListAsync();
            var totalRaised = donationsList.Sum(d => d.Amount);
            var goal = animal.MonthlyDonationGoal > 0 ? animal.MonthlyDonationGoal : 45000;
            var percentage = Math.Min(100, (int)Math.Round((totalRaised / goal) * 100));

            return Ok(new
            {
                success = true,
                message = "Сердечно благодарим за вашу поддержку и заботу о животных!",
                donation = new DonationResponseDto
                {
                    Id = donation.Id,
                    DonorName = donation.DonorName,
                    Amount = donation.Amount,
                    Target = donation.Target,
                    Message = donation.Message,
                    CreatedAt = donation.CreatedAt
                },
                summary = new DonationSummaryDto
                {
                    TotalRaised = totalRaised,
                    Goal = goal,
                    Percentage = percentage,
                    DonorsCount = donationsList.Count
                }
            });
        }
    }
}

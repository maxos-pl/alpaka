using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StavZooApp.Data;
using StavZooApp.DTOs;

namespace StavZooApp.Controllers.Api
{
    [ApiController]
    [Route("api/animals")]
    [Produces("application/json")]
    public class AnimalsApiController : ControllerBase
    {
        private readonly ZooDbContext _db;

        public AnimalsApiController(ZooDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Список всех животных зоопарка
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalSummaryDto>>> GetAnimals()
        {
            var animals = await _db.Animals
                .Include(a => a.Donations)
                .ToListAsync();

            var dtos = animals.Select(a => new AnimalSummaryDto
            {
                Id = a.Id,
                Slug = a.Slug,
                Name = a.Name,
                Species = a.Species,
                LatinName = a.LatinName,
                EnclosureNumber = a.EnclosureNumber,
                Status = a.Status,
                HeroImageUrl = a.HeroImageUrl,
                MonthlyDonationGoal = a.MonthlyDonationGoal,
                TotalDonations = a.Donations.Sum(d => d.Amount)
            }).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Подробная информация о конкретном животном по его кодовому имени (slug, например "alpaca")
        /// </summary>
        [HttpGet("{slug}")]
        public async Task<ActionResult<AnimalDetailDto>> GetAnimalBySlug(string slug)
        {
            var animal = await _db.Animals
                .Include(a => a.MediaItems)
                .Include(a => a.DiaryEntries.OrderByDescending(e => e.Date).Take(10))
                .Include(a => a.Donations)
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null)
            {
                return NotFound(new { message = $"Животное с идентификатором '{slug}' не найдено" });
            }

            var dto = new AnimalDetailDto
            {
                Id = animal.Id,
                Slug = animal.Slug,
                Name = animal.Name,
                Species = animal.Species,
                LatinName = animal.LatinName,
                Family = animal.Family,
                Origin = animal.Origin,
                EnclosureNumber = animal.EnclosureNumber,
                Status = animal.Status,
                DietSummary = animal.DietSummary,
                Description = animal.Description,
                CharacterTraits = animal.CharacterTraits,
                History = animal.History,
                HeroImageUrl = animal.HeroImageUrl,
                WebcamStreamUrl = animal.WebcamStreamUrl,
                MonthlyDonationGoal = animal.MonthlyDonationGoal,
                TotalDonations = animal.Donations.Sum(d => d.Amount),
                ArrivalDate = animal.ArrivalDate,
                BirthDate = animal.BirthDate,
                Media = animal.MediaItems.Select(m => new MediaDto
                {
                    Id = m.Id,
                    MediaType = m.MediaType,
                    MediaUrl = m.MediaUrl,
                    ThumbnailUrl = m.ThumbnailUrl,
                    Title = m.Title,
                    Description = m.Description
                }).ToList(),
                RecentDiaryEntries = animal.DiaryEntries.Select(e => new DiaryResponseDto
                {
                    Id = e.Id,
                    AnimalId = e.AnimalId,
                    AnimalName = animal.Name,
                    Date = e.Date,
                    Category = e.Category,
                    Title = e.Title,
                    Description = e.Description,
                    HealthStatus = e.HealthStatus,
                    DietDetails = e.DietDetails,
                    WeightKg = e.WeightKg,
                    TemperatureC = e.TemperatureC,
                    AuthorName = e.AuthorName,
                    CreatedAt = e.CreatedAt
                }).ToList()
            };

            return Ok(dto);
        }
    }
}

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
    [Route("stav-zoo/api/animals")]
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
        /// Подробная информация о конкретном животном по его slug (например "alpaca")
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
                return NotFound(new { message = $"Животное '{slug}' не найдено" });
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

        /// <summary>
        /// Паспорт и текстовое описание особи
        /// </summary>
        [HttpGet("{slug}/passport")]
        public async Task<IActionResult> GetPassport(string slug)
        {
            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());
            if (animal == null) return NotFound(new { message = "Особь не найдена" });

            return Ok(new
            {
                animal.Id,
                animal.Slug,
                animal.Name,
                animal.Species,
                animal.LatinName,
                animal.Family,
                animal.Origin,
                animal.EnclosureNumber,
                animal.Status,
                animal.BirthDate,
                animal.ArrivalDate,
                animal.Description,
                animal.History,
                animal.CharacterTraits,
                animal.DietSummary
            });
        }

        /// <summary>
        /// Фото и видео материалы особи
        /// </summary>
        [HttpGet("{slug}/media")]
        public async Task<IActionResult> GetMedia(string slug)
        {
            var animal = await _db.Animals
                .Include(a => a.MediaItems)
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null) return NotFound(new { message = "Особь не найдена" });

            var photos = animal.MediaItems.Where(m => m.MediaType == "photo").Select(m => new
            {
                m.Id,
                m.Title,
                m.Description,
                m.MediaUrl,
                m.ThumbnailUrl
            });

            var videos = animal.MediaItems.Where(m => m.MediaType == "video").Select(m => new
            {
                m.Id,
                m.Title,
                m.Description,
                m.MediaUrl
            });

            return Ok(new { animal.Name, animal.Species, photos, videos });
        }

        /// <summary>
        /// Онлайн веб-камера и расписание активности
        /// </summary>
        [HttpGet("{slug}/webcam")]
        public async Task<IActionResult> GetWebcam(string slug)
        {
            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());
            if (animal == null) return NotFound(new { message = "Особь не найдена" });

            return Ok(new
            {
                animal.Name,
                animal.Species,
                animal.EnclosureNumber,
                currentLocation = "Вольер №14 (Общий вид)",
                webcamStreamUrl = animal.WebcamStreamUrl,
                availableCameras = new[]
                {
                    new { id = 1, name = "Вольер №14 (Общий вид)", streamUrl = animal.WebcamStreamUrl },
                    new { id = 2, name = "Вольер №14 (Зона кормления)", streamUrl = "/images/alpaca-video.mp4" }
                },
                schedule = new[]
                {
                    "08:30 - 09:30 — Утреннее кормление сеном и сочными овощами.",
                    "11:00 - 13:00 — Прогулка и активные игры на открытой площадке.",
                    "14:30 - 15:30 — Дневной уход, тренинг, угощение морковью.",
                    "18:00 - 19:00 — Вечернее кормление."
                }
            });
        }
    }
}

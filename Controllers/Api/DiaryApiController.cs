using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StavZooApp.Data;
using StavZooApp.DTOs;
using StavZooApp.Models;

namespace StavZooApp.Controllers.Api
{
    [ApiController]
    [Route("api/animals/{slug}/diary")]
    [Produces("application/json")]
    public class DiaryApiController : ControllerBase
    {
        private readonly ZooDbContext _db;

        public DiaryApiController(ZooDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Получение записей дневника животного с возможностью фильтрации по категории
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaryResponseDto>>> GetDiary(
            string slug,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null)
        {
            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());
            if (animal == null)
            {
                return NotFound(new { message = $"Животное '{slug}' не найдено" });
            }

            var query = _db.DiaryEntries.Where(d => d.AnimalId == animal.Id);

            if (!string.IsNullOrWhiteSpace(category) && category != "Все")
            {
                query = query.Where(d => d.Category.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(d => d.Title.ToLower().Contains(s) || d.Description.ToLower().Contains(s) || (d.DietDetails != null && d.DietDetails.ToLower().Contains(s)));
            }

            var entries = await query
                .OrderByDescending(d => d.Date)
                .Select(d => new DiaryResponseDto
                {
                    Id = d.Id,
                    AnimalId = d.AnimalId,
                    AnimalName = animal.Name,
                    Date = d.Date,
                    Category = d.Category,
                    Title = d.Title,
                    Description = d.Description,
                    HealthStatus = d.HealthStatus,
                    DietDetails = d.DietDetails,
                    WeightKg = d.WeightKg,
                    TemperatureC = d.TemperatureC,
                    AuthorName = d.AuthorName,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            return Ok(entries);
        }

        /// <summary>
        /// Получение отдельной записи дневника по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryResponseDto>> GetDiaryEntry(string slug, int id)
        {
            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());
            if (animal == null)
            {
                return NotFound(new { message = $"Животное '{slug}' не найдено" });
            }

            var entry = await _db.DiaryEntries.FirstOrDefaultAsync(d => d.Id == id && d.AnimalId == animal.Id);
            if (entry == null)
            {
                return NotFound(new { message = "Запись дневника не найдена" });
            }

            return Ok(new DiaryResponseDto
            {
                Id = entry.Id,
                AnimalId = entry.AnimalId,
                AnimalName = animal.Name,
                Date = entry.Date,
                Category = entry.Category,
                Title = entry.Title,
                Description = entry.Description,
                HealthStatus = entry.HealthStatus,
                DietDetails = entry.DietDetails,
                WeightKg = entry.WeightKg,
                TemperatureC = entry.TemperatureC,
                AuthorName = entry.AuthorName,
                CreatedAt = entry.CreatedAt
            });
        }

        /// <summary>
        /// Добавление новой записи в дневник (только для авторизованных работников зоопарка)
        /// </summary>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Cookies,Bearer", Roles = "Employee,Admin,Veterinarian")]
        public async Task<ActionResult<DiaryResponseDto>> CreateDiaryEntry(string slug, [FromBody] DiaryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());
            if (animal == null)
            {
                return NotFound(new { message = $"Животное '{slug}' не найдено" });
            }

            var authorName = User.Identity?.Name ?? "Сотрудник зоопарка";
            var position = User.FindFirst("Position")?.Value;
            if (!string.IsNullOrEmpty(position))
            {
                authorName += $" ({position})";
            }

            int? authorId = null;
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedId))
            {
                authorId = parsedId;
            }

            var entry = new AnimalDiaryEntry
            {
                AnimalId = animal.Id,
                Date = dto.Date ?? DateTime.UtcNow,
                Category = dto.Category,
                Title = dto.Title,
                Description = dto.Description,
                HealthStatus = string.IsNullOrWhiteSpace(dto.HealthStatus) ? "Отличное" : dto.HealthStatus,
                DietDetails = dto.DietDetails,
                WeightKg = dto.WeightKg,
                TemperatureC = dto.TemperatureC,
                AuthorName = authorName,
                AuthorUserId = authorId,
                CreatedAt = DateTime.UtcNow
            };

            _db.DiaryEntries.Add(entry);
            await _db.SaveChangesAsync();

            var response = new DiaryResponseDto
            {
                Id = entry.Id,
                AnimalId = entry.AnimalId,
                AnimalName = animal.Name,
                Date = entry.Date,
                Category = entry.Category,
                Title = entry.Title,
                Description = entry.Description,
                HealthStatus = entry.HealthStatus,
                DietDetails = entry.DietDetails,
                WeightKg = entry.WeightKg,
                TemperatureC = entry.TemperatureC,
                AuthorName = entry.AuthorName,
                CreatedAt = entry.CreatedAt
            };

            return Created($"/api/animals/{slug}/diary/{entry.Id}", response);
        }

        /// <summary>
        /// Редактирование записи дневника (только для авторизованных работников зоопарка)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Cookies,Bearer", Roles = "Employee,Admin,Veterinarian")]
        public async Task<IActionResult> UpdateDiaryEntry(string slug, int id, [FromBody] DiaryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());
            if (animal == null)
            {
                return NotFound(new { message = $"Животное '{slug}' не найдено" });
            }

            var entry = await _db.DiaryEntries.FirstOrDefaultAsync(d => d.Id == id && d.AnimalId == animal.Id);
            if (entry == null)
            {
                return NotFound(new { message = "Запись дневника не найдена" });
            }

            entry.Category = dto.Category;
            entry.Title = dto.Title;
            entry.Description = dto.Description;
            entry.HealthStatus = dto.HealthStatus;
            entry.DietDetails = dto.DietDetails;
            entry.WeightKg = dto.WeightKg;
            entry.TemperatureC = dto.TemperatureC;
            if (dto.Date.HasValue)
            {
                entry.Date = dto.Date.Value;
            }

            await _db.SaveChangesAsync();

            return Ok(entry);
        }

        /// <summary>
        /// Удаление записи дневника (только для авторизованных работников зоопарка)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Cookies,Bearer", Roles = "Employee,Admin,Veterinarian")]
        public async Task<IActionResult> DeleteDiaryEntry(string slug, int id)
        {
            var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());
            if (animal == null)
            {
                return NotFound(new { message = $"Животное '{slug}' не найдено" });
            }

            var entry = await _db.DiaryEntries.FirstOrDefaultAsync(d => d.Id == id && d.AnimalId == animal.Id);
            if (entry == null)
            {
                return NotFound(new { message = "Запись дневника не найдена" });
            }

            _db.DiaryEntries.Remove(entry);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Запись успешно удалена" });
        }
    }
}

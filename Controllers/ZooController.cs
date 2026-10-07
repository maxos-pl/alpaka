using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StavZooApp.Data;

namespace StavZooApp.Controllers
{
    public class ZooController : Controller
    {
        private readonly ZooDbContext _db;

        public ZooController(ZooDbContext db)
        {
            _db = db;
        }

        // Главная страница: /stav-zoo
        [Route("stav-zoo")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var animals = await _db.Animals
                .Include(a => a.Donations)
                .ToListAsync();

            return View(animals);
        }

        // 1. Паспорт особи и текстовое описание: /stav-zoo/{slug}
        [Route("stav-zoo/{slug}")]
        [HttpGet]
        public async Task<IActionResult> Animal(string slug = "alpaca")
        {
            var animal = await _db.Animals
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null) return NotFound("Животное не найдено");
            return View(animal);
        }

        // 2. Фото и видео материалы: /stav-zoo/{slug}/media
        [Route("stav-zoo/{slug}/media")]
        [HttpGet]
        public async Task<IActionResult> Media(string slug = "alpaca")
        {
            var animal = await _db.Animals
                .Include(a => a.MediaItems)
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null) return NotFound("Животное не найдено");
            return View(animal);
        }

        // 3. Наблюдение через веб-камеру: /stav-zoo/{slug}/webcam
        [Route("stav-zoo/{slug}/webcam")]
        [HttpGet]
        public async Task<IActionResult> Webcam(string slug = "alpaca")
        {
            var animal = await _db.Animals
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null) return NotFound("Животное не найдено");
            return View(animal);
        }

        // 4. Дневник / история особи и популяции: /stav-zoo/{slug}/diary
        [Route("stav-zoo/{slug}/diary")]
        [HttpGet]
        public async Task<IActionResult> Diary(string slug = "alpaca", string? category = null, string? search = null)
        {
            var animal = await _db.Animals
                .Include(a => a.DiaryEntries.OrderByDescending(d => d.Date))
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null) return NotFound("Животное не найдено");
            return View(animal);
        }

        // 5. Донат на корм / лечение: /stav-zoo/{slug}/donate
        [Route("stav-zoo/{slug}/donate")]
        [HttpGet]
        public async Task<IActionResult> Donate(string slug = "alpaca")
        {
            var animal = await _db.Animals
                .Include(a => a.Donations.OrderByDescending(d => d.CreatedAt))
                .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (animal == null) return NotFound("Животное не найдено");
            return View(animal);
        }

        // Редирект с корня / на /stav-zoo
        [Route("")]
        [HttpGet]
        public IActionResult Root()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}

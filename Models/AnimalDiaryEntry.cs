using System;

namespace StavZooApp.Models
{
    public class AnimalDiaryEntry
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Category { get; set; } = string.Empty; // "Кормёжка", "Спаривание", "Потомство", "Ветеринария", "Уход и стрижка", "Поведение"
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HealthStatus { get; set; } = "Отличное"; // "Отличное", "Хорошее", "Удовлетворительное", "Под наблюдением"
        public string? DietDetails { get; set; }
        public double? WeightKg { get; set; }
        public double? TemperatureC { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int? AuthorUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

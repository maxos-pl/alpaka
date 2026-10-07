using System;
using System.ComponentModel.DataAnnotations;

namespace StavZooApp.DTOs
{
    public class DiaryCreateDto
    {
        [Required(ErrorMessage = "Категория обязательна")]
        public string Category { get; set; } = string.Empty; // "Кормёжка", "Спаривание", "Потомство", "Ветеринария", "Уход и стрижка", "Поведение"

        [Required(ErrorMessage = "Заголовок обязателен")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; } = string.Empty;

        public string HealthStatus { get; set; } = "Отличное";

        public string? DietDetails { get; set; }

        public double? WeightKg { get; set; }

        public double? TemperatureC { get; set; }

        public DateTime? Date { get; set; }
    }

    public class DiaryUpdateDto : DiaryCreateDto
    {
        public int Id { get; set; }
    }

    public class DiaryResponseDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HealthStatus { get; set; } = string.Empty;
        public string? DietDetails { get; set; }
        public double? WeightKg { get; set; }
        public double? TemperatureC { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

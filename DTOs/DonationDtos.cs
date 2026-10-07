using System;
using System.ComponentModel.DataAnnotations;

namespace StavZooApp.DTOs
{
    public class DonationCreateDto
    {
        [Required]
        public string AnimalSlug { get; set; } = "alpaca";

        [Required(ErrorMessage = "Укажите имя или псевдоним")]
        [StringLength(100)]
        public string DonorName { get; set; } = "Друг зоопарка";

        [Required(ErrorMessage = "Укажите сумму")]
        [Range(10, 1000000, ErrorMessage = "Сумма доната должна быть от 10 ₽")]
        public decimal Amount { get; set; }

        public string Target { get; set; } = "Корм и лакомства";

        [StringLength(500)]
        public string? Message { get; set; }
    }

    public class DonationResponseDto
    {
        public int Id { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Target { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DonationSummaryDto
    {
        public decimal TotalRaised { get; set; }
        public decimal Goal { get; set; }
        public int Percentage { get; set; }
        public int DonorsCount { get; set; }
    }
}

using System;

namespace StavZooApp.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public string DonorName { get; set; } = "Анонимный друг";
        public decimal Amount { get; set; }
        public string Target { get; set; } = "Корм и лакомства"; // "Корм и лакомства", "Медицинский уход и витамины", "Обустройство вольера", "Свободный донат"
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

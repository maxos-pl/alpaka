using System;
using System.Collections.Generic;

namespace StavZooApp.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty; // e.g., "alpaca"
        public string Name { get; set; } = string.Empty; // e.g., "Пако и Лола"
        public string Species { get; set; } = string.Empty; // e.g., "Альпака"
        public string LatinName { get; set; } = string.Empty; // e.g., "Vicugna pacos"
        public string Family { get; set; } = string.Empty; // e.g., "Верблюдовые"
        public string Origin { get; set; } = string.Empty; // e.g., "Анды (Южная Америка)"
        public string EnclosureNumber { get; set; } = string.Empty; // e.g., "Вольер № 14"
        public string Status { get; set; } = string.Empty; // e.g., "Здоровы, активны"
        public string DietSummary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CharacterTraits { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public string WebcamStreamUrl { get; set; } = string.Empty;
        public decimal MonthlyDonationGoal { get; set; } = 45000;
        public DateTime ArrivalDate { get; set; }
        public DateTime BirthDate { get; set; }

        public List<AnimalMedia> MediaItems { get; set; } = new();
        public List<AnimalDiaryEntry> DiaryEntries { get; set; } = new();
        public List<Donation> Donations { get; set; } = new();
    }
}

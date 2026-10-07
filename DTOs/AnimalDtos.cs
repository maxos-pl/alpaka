using System;
using System.Collections.Generic;

namespace StavZooApp.DTOs
{
    public class AnimalSummaryDto
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string LatinName { get; set; } = string.Empty;
        public string EnclosureNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public decimal TotalDonations { get; set; }
        public decimal MonthlyDonationGoal { get; set; }
    }

    public class AnimalDetailDto
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string LatinName { get; set; } = string.Empty;
        public string Family { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string EnclosureNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DietSummary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CharacterTraits { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public string WebcamStreamUrl { get; set; } = string.Empty;
        public decimal MonthlyDonationGoal { get; set; }
        public decimal TotalDonations { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime BirthDate { get; set; }
        public List<MediaDto> Media { get; set; } = new();
        public List<DiaryResponseDto> RecentDiaryEntries { get; set; } = new();
    }

    public class MediaDto
    {
        public int Id { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string MediaUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;

namespace DataAccessLayer.DTO.Dashboard
{
    public class DonationActivityDTO
    {
        public int TotalDonations { get; set; }
        public int CompletedDonations { get; set; }
        public int ScheduledDonations { get; set; }
        public decimal SuccessRate { get; set; } // Percentage of successful donations
        public decimal TotalVolumeCollected { get; set; } // In milliliters
        public List<RecentDonationDTO> RecentDonations { get; set; }
        public List<DonationsByDateDTO> DonationTrend { get; set; }
        public Dictionary<int, int> DonationsByType { get; set; } // DonationTypeId -> Count
    }

    public class RecentDonationDTO
    {
        public int RecordId { get; set; }
        public string DonorName { get; set; }
        public DateTime DonationDateTime { get; set; }
        public decimal Volume { get; set; }
        public string BloodTypeName { get; set; }
        public int? DonationTypeId { get; set; }
        public string DonationTypeName { get; set; }
    }

    public class DonationsByDateDTO
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public decimal TotalVolume { get; set; }
    }
}
using System;

namespace DataAccessLayer.DTO.Dashboard
{
    public class DashboardSummaryDTO
    {
        public int TotalDonors { get; set; }
        public int EligibleDonors { get; set; }
        public int AvailableBloodUnits { get; set; }
        public int PendingRequests { get; set; }
        public int ScheduledDonations { get; set; }
        public int CompletedDonationsThisMonth { get; set; }
        public decimal TotalBloodVolumeAvailable { get; set; }
        public decimal FulfillmentRate { get; set; }
    }
}
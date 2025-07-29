using System;
using System.Collections.Generic;

namespace DataAccessLayer.DTO.Dashboard
{
    public class BloodRequestsDTO
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int CompletedRequests { get; set; }
        public decimal TotalVolumeRequested { get; set; }
        public decimal TotalVolumeUnfulfilled { get; set; }
        public decimal FulfillmentRate { get; set; } 
        public Dictionary<int, int> RequestsByUrgency { get; set; } 
        public Dictionary<int, int> RequestsByBloodType { get; set; } 
    }
}
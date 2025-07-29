using System;
using System.Collections.Generic;

namespace DataAccessLayer.DTO.Dashboard
{
    public class HospitalActivityDTO
    {
        public int TotalHospitals { get; set; }
        public int ActiveHospitals { get; set; } 
        public List<HospitalRequestSummaryDTO> TopRequestingHospitals { get; set; }
        public Dictionary<int, HospitalFulfillmentDTO> HospitalFulfillmentRates { get; set; } 
    }

    public class HospitalRequestSummaryDTO
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int RequestCount { get; set; }
        public decimal TotalVolumeRequested { get; set; }
    }

    public class HospitalFulfillmentDTO
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int TotalRequests { get; set; }
        public int FulfilledRequests { get; set; }
        public int UnfulfilledRequests { get; set; }
        public decimal FulfillmentRate { get; set; } 
    }
}
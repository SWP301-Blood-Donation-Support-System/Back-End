using System;
using System.Collections.Generic;

namespace DataAccessLayer.DTO.Dashboard
{
    public class DonorStatisticsDTO
    {
        public int TotalDonors { get; set; }
        public int EligibleDonors { get; set; }
        public int NewDonorsThisMonth { get; set; }
        public Dictionary<int, int> DonorsByBloodType { get; set; } 
        public Dictionary<int, int> DonorsByAvailability { get; set; } 
        public List<DonorTrendDTO> RegistrationTrend { get; set; } 
    }

    public class DonorTrendDTO
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
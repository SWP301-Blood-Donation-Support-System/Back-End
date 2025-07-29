using System;
using System.Collections.Generic;

namespace DataAccessLayer.DTO.Dashboard
{
    public class BloodInventoryDTO
    {
        public int TotalUnits { get; set; }
        public int AvailableUnits { get; set; }
        public int AssignedUnits { get; set; }
        public int ExpiredUnits { get; set; }
        public int UnusableUnits { get; set; }
        public int ExpiringWithinWeek { get; set; }
        public Dictionary<int, int> UnitsByBloodType { get; set; } 
        public Dictionary<int, int> UnitsByComponent { get; set; } 
        public Dictionary<int, int> UnitsByStatus { get; set; } 
    }
}
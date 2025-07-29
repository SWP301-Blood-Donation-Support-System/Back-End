using System;
using System.Collections.Generic;

namespace DataAccessLayer.DTO.Dashboard
{
    public class SystemHealthDTO
    {
        public Dictionary<int, int> UsersByRole { get; set; } 
        public int ActiveUsersLast30Days { get; set; }
    }
}
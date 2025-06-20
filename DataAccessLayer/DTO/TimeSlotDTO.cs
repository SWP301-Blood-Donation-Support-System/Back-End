using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class TimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public string TimeSlotName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Capacity { get; set; }
        public bool IsDeleted { get; set; }
    }
}

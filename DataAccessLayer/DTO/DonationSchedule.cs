using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class DonationScheduleDTO
    {
        public int ScheduleId { get; set; }
        
        public DateTime? ScheduleDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class DonationRegistrationDTO
    {
        public int DonorId { get; set; }
        public int ScheduleId { get; set; }
        public int TimeSlotId { get; set; }
    }
}

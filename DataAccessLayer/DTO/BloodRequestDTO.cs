using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class BloodRequestDTO
    {
        public int RequestingStaffId { get; set; }

        public int BloodTypeId { get; set; }

        public int BloodComponentId { get; set; }

        public decimal Volume { get; set; }

        public DateTime RequiredDateTime { get; set; }

        public int RequestStatusId { get; set; }

        public int UrgencyId { get; set; }

        public string Note { get; set; }
    }
}

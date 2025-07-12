using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class BloodUnitResponseDTO
    {
        public int BloodUnitId { get; set; }
        public int DonationRecordId { get; set; }
        public string BloodTypeName { get; set; }
        public string ComponentName { get; set; }
        public DateTime? CollectedDateTime { get; set; }
        public DateTime? ExpiryDateTime { get; set; }
        public decimal Volume { get; set; }
        public string StatusName { get; set; }
        public string DonorName { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}

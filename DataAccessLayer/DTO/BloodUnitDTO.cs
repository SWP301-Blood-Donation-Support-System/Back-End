using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class BloodUnitDTO
    {
        public int DonationRecordId { get; set; }

        public int BloodTypeId { get; set; }

        public int? ComponentId { get; set; }

        public DateTime? CollectedDateTime { get; set; }

        public decimal? Volume { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class DonationValidationDTO
    {
        public int UserId { get; set; }
        public int DonationRecordId { get; set; }
        // Các thông tin bổ sung nếu cần
    }
}

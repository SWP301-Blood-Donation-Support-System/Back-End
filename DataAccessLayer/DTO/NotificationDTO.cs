using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class NotificationDTO
    {
        public int NotificationTypeId { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }

    }
}

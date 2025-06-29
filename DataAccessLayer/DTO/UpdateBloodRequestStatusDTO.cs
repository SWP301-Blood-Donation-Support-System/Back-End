using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class UpdateBloodRequestStatusDTO
    {
        public int RequestId { get; set; }
        public int RequestStatusId { get; set; }
    }
}

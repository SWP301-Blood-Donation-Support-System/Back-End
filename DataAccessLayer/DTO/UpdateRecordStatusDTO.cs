using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class UpdateRecordStatusDTO
    {
        public int RecordId {  get; set; }
        public int StatusId { get; set; }
    }
}

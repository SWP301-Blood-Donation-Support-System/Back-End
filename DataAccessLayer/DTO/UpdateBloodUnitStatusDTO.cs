using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class UpdateBloodUnitStatusDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Mã đơn vị máu không hợp lệ.")]
        public int unitId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Mã trạng thái mới không hợp lệ.")]
        public int bloodUnitStatusId { get; set; }
    }
}

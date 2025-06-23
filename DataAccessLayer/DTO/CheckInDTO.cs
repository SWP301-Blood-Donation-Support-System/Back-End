using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class CheckInDTO
    {
        [Required(ErrorMessage = "Số CCCD/CMND là bắt buộc.")]
        [StringLength(12, MinimumLength = 9)]
        public string NationalId { get; set; }
    }
}

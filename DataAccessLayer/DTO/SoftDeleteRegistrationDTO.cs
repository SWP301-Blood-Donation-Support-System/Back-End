using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class SoftDeleteRegistrationDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int RegistrationId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(16, ErrorMessage = "Username cannot be longer than 50 characters.",MinimumLength = 4)]
        public string Username { get; set; } = null!;


        public string PasswordHash { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string? NationalId { get; set; }
        public string? FullName { get; set; }

       }
}

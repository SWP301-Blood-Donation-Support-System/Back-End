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
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; } = null!;
        [Required(ErrorMessage = "Username is required.")]
        //[StringLength(16, ErrorMessage = "Username cannot be longer than 16 characters.", MinimumLength = 4)]
        public string Username { get; set; }
        //[Required(ErrorMessage = "Full Name is required.")]
        //public string? FullName { get; set; }
        //[Required(ErrorMessage = "Phone Number is required.")]
        //public string? PhoneNumber { get; set; }
        //[Required(ErrorMessage = "National Id is required.")]
        //public string? NationalId { get; set; }
        //[Required(ErrorMessage = "DateOfBirth is required.")]
        //public DateOnly? DateOfBirth { get; set; }
    }

}

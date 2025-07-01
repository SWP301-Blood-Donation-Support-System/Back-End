using Microsoft.AspNetCore.Http;
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
        public string Username { get; set; } = null!;
        public IFormFile? UserImage { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }
}

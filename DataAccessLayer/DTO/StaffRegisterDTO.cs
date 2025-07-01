using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.DTO
{
    public class StaffRegisterDTO
    {
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public string StaffCode { get; set; } = null!;
    }
}

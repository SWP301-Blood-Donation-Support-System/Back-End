using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class RegisterDTO
    {
        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Email { get; set; }
        public string? NationalId { get; set; }
        public string? FullName { get; set; }
        public int RoleId { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; }
    }

    public class WelcomeEmailRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        public string? UserName { get; set; }
    }
}

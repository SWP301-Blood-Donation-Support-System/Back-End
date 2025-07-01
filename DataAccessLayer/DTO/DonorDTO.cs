using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.DTO
{
    public class DonorDTO
    {

        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? NationalId { get; set; }

        public string? Address { get; set; }

        public int? GenderId { get; set; }

        public int? OccupationId { get; set; }

        public int? BloodTypeId { get; set; }
        
        public IFormFile? UserImage { get; set; }
    }
}

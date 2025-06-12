using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class UserDTO
    {

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public int RoleId { get; set; }

        public string? StaffCode { get; set; }

        public string? NationalId { get; set; }

        public string? Address { get; set; }

        public int? GenderId { get; set; }

        public int? OccupationId { get; set; }

        public int? BloodTypeId { get; set; }

        public DateTime? LastDonationDate { get; set; }

        public DateTime? NextEligibleDonationDate { get; set; }

        public int? DonationCount { get; set; }

        public int DonationAvailabilityId { get; set; }


    }
}

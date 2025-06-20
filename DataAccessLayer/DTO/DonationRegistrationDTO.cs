using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class DonationRegistrationDTO
    {
        public int DonorId { get; set; }
        public int ScheduleId { get; set; }
        public int TimeSlotId { get; set; }
    }

    public class DonationRegistrationResponseDTO
    {
        public int RegistrationId { get; set; }
        public int DonorId { get; set; }
        public int ScheduleId { get; set; }
        public int TimeSlotId { get; set; }
        public int RegistrationStatusId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public DonorBasicInfoDTO Donor { get; set; }
        public TimeSlotDTO TimeSlot { get; set; }
        public RegistrationStatusDTO Status { get; set; }
    }
    
    public class DonorBasicInfoDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalId { get; set; }
        public int? BloodTypeId { get; set; }
        public string Address { get; set; }
    }
    
    public class RegistrationStatusDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }
}

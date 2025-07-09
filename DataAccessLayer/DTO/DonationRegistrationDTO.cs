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
        public int? TimeSlotId { get; set; }
    }

    public class DonationRegistrationResponseDTO
    {
        public int RegistrationId { get; set; }
        public int DonorId { get; set; }
        public int ScheduleId { get; set; }
        public int? TimeSlotId { get; set; }
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

    // DTO for donation registration thank you email
    public class DonationRegistrationEmailInfoDTO
    {
        public int RegistrationId { get; set; }
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }
        public string DonorPhone { get; set; }
        public string DonorNationalId { get; set; }
        public string BloodType { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string ScheduleLocation { get; set; }
        public string TimeSlotName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RegistrationCode { get; set; }
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
    
    public class TimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public string TimeSlotName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Capacity { get; set; }
        public bool IsDeleted { get; set; }
    }
    
    public class RegistrationStatusDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    /// <summary>
    /// DTO cho danh sách ng??i có th? hi?n máu trong 3 ngày t?i
    /// </summary>
    public class UpcomingEligibleDonorsDTO
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BloodTypeName { get; set; }
        public DateTime? NextEligibleDonationDate { get; set; }
        public int DaysUntilEligible { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO cho request g?i thông báo nh?c nh? hàng lo?t
    /// </summary>
    public class BulkReminderRequestDTO
    {
        public List<int> UserIds { get; set; } = new List<int>();
        public string? CustomMessage { get; set; }
        public bool SendEmail { get; set; } = true;
        public bool SendNotification { get; set; } = true;
    }

    /// <summary>
    /// DTO cho response c?a bulk reminder
    /// </summary>
    public class BulkReminderResponseDTO
    {
        public int TotalTargetUsers { get; set; }
        public int SuccessfulNotifications { get; set; }
        public int FailedNotifications { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public DateTime ProcessedAt { get; set; }
        public string? ProcessedBy { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách ng??i dùng có l?ch hi?n vào ngày mai
    /// </summary>
    public class TomorrowDonationScheduleDTO
    {
        public int RegistrationId { get; set; }
        public int DonorId { get; set; }
        public string? DonorName { get; set; }
        public string? DonorEmail { get; set; }
        public string? DonorPhone { get; set; }
        public string? BloodTypeName { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? TimeSlotName { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Location { get; set; }
        public string? HospitalName { get; set; }
        public string? HospitalAddress { get; set; }
        public int RegistrationStatusId { get; set; }
        public string? StatusName { get; set; }
    }

    /// <summary>
    /// DTO cho response c?a auto reminder job
    /// </summary>
    public class AutoReminderJobResponseDTO
    {
        public int TotalUpcomingDonations { get; set; }
        public int SuccessfulNotifications { get; set; }
        public int FailedNotifications { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public DateTime ProcessedAt { get; set; }
        public string? ProcessedBy { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }
}
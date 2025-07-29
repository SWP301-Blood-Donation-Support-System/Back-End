using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    /// <summary>
    /// DTO cho danh s�ch ng??i c� th? hi?n m�u trong 3 ng�y t?i
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
    /// DTO cho request g?i th�ng b�o nh?c nh? h�ng lo?t
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
}
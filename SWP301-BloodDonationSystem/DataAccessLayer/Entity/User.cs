using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; }

    public string FullName { get; set; }

    public int RoleId { get; set; }

    public string StaffCode { get; set; }

    public string NationalId { get; set; }

    public string? Address { get; set; }

    public int? GenderId { get; set; }

    public int? OccupationId { get; set; }

    public int BloodTypeId { get; set; }

    public DateTime LastDonationDate { get; set; }

    public DateTime NextEligibleDonationDate { get; set; }

    public int? DonationCount { get; set; }

    public int? DonationAvailability { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public virtual BloodType? BloodType { get; set; }

    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();

    public virtual DonationAvailability? DonationAvailabilityNavigation { get; set; }

    public virtual ICollection<DonationRecord> DonationRecordDonors { get; set; } = new List<DonationRecord>();

    public virtual ICollection<DonationRecord> DonationRecordProcessingStaffs { get; set; } = new List<DonationRecord>();

    public virtual Gender? Gender { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Occupation? Occupation { get; set; }

    public virtual Role Role { get; set; } = null!;
}

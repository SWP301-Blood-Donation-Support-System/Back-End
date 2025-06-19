using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

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

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public virtual BloodType? BloodType { get; set; }

    public virtual DonationAvailability DonationAvailability { get; set; } = null!;

    public virtual ICollection<DonationRegistration> DonationRegistrations { get; set; } = new List<DonationRegistration>();

    public virtual ICollection<DonationValidation> DonationValidations { get; set; } = new List<DonationValidation>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Gender? Gender { get; set; }

    public virtual Occupation? Occupation { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}

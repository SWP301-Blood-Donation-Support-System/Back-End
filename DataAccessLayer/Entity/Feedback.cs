using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int RegistrationId { get; set; }

    public string FeedbackInfo { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual DonationRegistration Registration { get; set; } = null!;
}

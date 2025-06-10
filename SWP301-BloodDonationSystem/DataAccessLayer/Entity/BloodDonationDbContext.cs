using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Entity;

public partial class BloodDonationDbContext : DbContext
{
    public BloodDonationDbContext()
    {
    }

    public BloodDonationDbContext(DbContextOptions<BloodDonationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleCategory> ArticleCategories { get; set; }

    public virtual DbSet<ArticleStatus> ArticleStatuses { get; set; }

    public virtual DbSet<BloodComponent> BloodComponents { get; set; }

    public virtual DbSet<BloodRequest> BloodRequests { get; set; }

    public virtual DbSet<BloodRequestStatus> BloodRequestStatuses { get; set; }

    public virtual DbSet<BloodTestResult> BloodTestResults { get; set; }

    public virtual DbSet<BloodType> BloodTypes { get; set; }

    public virtual DbSet<BloodUnit> BloodUnits { get; set; }

    public virtual DbSet<BloodUnitStatus> BloodUnitStatuses { get; set; }

    public virtual DbSet<DonationAppointment> DonationAppointments { get; set; }

    public virtual DbSet<DonationAvailability> DonationAvailabilities { get; set; }

    public virtual DbSet<DonationRecord> DonationRecords { get; set; }

    public virtual DbSet<DonationType> DonationTypes { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Occupation> Occupations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<Urgency> Urgencies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);user=sa;password=12345;Database=BloodDonationDB;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentStatus>(entity =>
        {
            entity.HasKey(e => e.AppointmentStatusId).HasName("PK__Appointm__A619B640D69034FF");

            entity.ToTable("AppointmentStatus");

            entity.Property(e => e.AppointmentStatusId).HasColumnName("AppointmentStatusID");
            entity.Property(e => e.AppointmentStatusName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Article__9C6270C80D28A1AF");

            entity.ToTable("Article");

            entity.HasIndex(e => e.AuthorUserId, "IX_Article_AuthorUserID");

            entity.HasIndex(e => e.ArticleCategoryId, "IX_Article_CategoryID");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.ArticleCategoryId).HasColumnName("ArticleCategoryID");
            entity.Property(e => e.AuthorUserId).HasColumnName("AuthorUserID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Picture).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ArticleCategory).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticleCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Article__Article__55209ACA");

            entity.HasOne(d => d.ArticleStatusNavigation).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticleStatus)
                .HasConstraintName("FK_Article_ArticleStatus");

            entity.HasOne(d => d.AuthorUser).WithMany(p => p.Articles)
                .HasForeignKey(d => d.AuthorUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Article__AuthorU__542C7691");
        });

        modelBuilder.Entity<ArticleCategory>(entity =>
        {
            entity.HasKey(e => e.ArticleCategoryId).HasName("PK__ArticleC__E0B60963730E1355");

            entity.ToTable("ArticleCategory");

            entity.Property(e => e.ArticleCategoryId).HasColumnName("ArticleCategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<ArticleStatus>(entity =>
        {
            entity.HasKey(e => e.ArticleStatusId).HasName("PK__ArticleS__3F0E2D6B98B2CF53");

            entity.ToTable("ArticleStatus");

            entity.Property(e => e.ArticleStatusId).HasColumnName("ArticleStatusID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<BloodComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__BloodCom__D79CF02E43A9345D");

            entity.ToTable("BloodComponent");

            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.ComponentName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<BloodRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__BloodReq__33A8519A6A24BEB8");

            entity.ToTable("BloodRequest");

            entity.HasIndex(e => e.BloodTypeId, "IX_BloodRequest_BloodTypeID");

            entity.HasIndex(e => e.RequestingStaffId, "IX_BloodRequest_RequestingStaffID");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.BloodComponentId).HasColumnName("BloodComponentID");
            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Quantity).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.RequestDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RequestStatusId).HasColumnName("RequestStatusID");
            entity.Property(e => e.RequestingStaffId).HasColumnName("RequestingStaffID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UrgencyId).HasColumnName("UrgencyID");

            entity.HasOne(d => d.BloodComponent).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.BloodComponentId)
                .HasConstraintName("FK__BloodRequ__Blood__7C4F7684");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodRequ__Blood__7B5B524B");

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.RequestStatusId)
                .HasConstraintName("FK__BloodRequ__Reque__7E37BEF6");

            entity.HasOne(d => d.RequestingStaff).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.RequestingStaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodRequ__Reque__7D439ABD");

            entity.HasOne(d => d.Urgency).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.UrgencyId)
                .HasConstraintName("FK__BloodRequ__Urgen__7F2BE32F");
        });

        modelBuilder.Entity<BloodRequestStatus>(entity =>
        {
            entity.HasKey(e => e.BloodRequestStatusId).HasName("PK__BloodReq__F73749E58BC0AFD2");

            entity.ToTable("BloodRequestStatus");

            entity.Property(e => e.BloodRequestStatusId).HasColumnName("BloodRequestStatusID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<BloodTestResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__BloodTes__97690228A38BACDD");

            entity.ToTable("BloodTestResult");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ResultName).HasMaxLength(100);
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId).HasName("PK__BloodTyp__B489BA4399B13F4C");

            entity.ToTable("BloodType");

            entity.HasIndex(e => e.BloodTypeName, "UQ__BloodTyp__3323606BE136806A").IsUnique();

            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.BloodTypeName).HasMaxLength(10);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.BloodUnitId).HasName("PK__BloodUni__AC1C2FABB7747114");

            entity.ToTable("BloodUnit");

            entity.HasIndex(e => e.BloodTypeId, "IX_BloodUnit_BloodTypeID");

            entity.HasIndex(e => e.DonationRecordId, "IX_BloodUnit_DonationRecordID");

            entity.HasIndex(e => e.ExpiryDateTime, "IX_BloodUnit_ExpiryDateTime");

            entity.Property(e => e.BloodUnitId).HasColumnName("BloodUnitID");
            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.BloodUnitStatusId).HasColumnName("BloodUnitStatusID");
            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DonationRecordId).HasColumnName("DonationRecordID");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Volume).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__Blood__00200768");

            entity.HasOne(d => d.BloodUnitStatus).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.BloodUnitStatusId)
                .HasConstraintName("FK__BloodUnit__Blood__01142BA1");

            entity.HasOne(d => d.Component).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("FK__BloodUnit__Compo__02084FDA");

            entity.HasOne(d => d.DonationRecord).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.DonationRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__Donat__01F34141");
        });

        modelBuilder.Entity<BloodUnitStatus>(entity =>
        {
            entity.HasKey(e => e.BloodUnitStatusId).HasName("PK__BloodUni__D4B59B31F628ED40");

            entity.ToTable("BloodUnitStatus");

            entity.Property(e => e.BloodUnitStatusId).HasColumnName("BloodUnitStatusID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<DonationAppointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Donation__8ECDFCA20869DFC6");

            entity.ToTable("DonationAppointment");

            entity.HasIndex(e => e.DonorId, "IX_DonationAppointment_DonorID");

            entity.HasIndex(e => e.ScheduledDateTime, "IX_DonationAppointment_ScheduledDateTime");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentStatusId).HasColumnName("AppointmentStatusID");
            entity.Property(e => e.BookingCode).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DonationTypeId).HasColumnName("DonationTypeID");
            entity.Property(e => e.DonorId).HasColumnName("DonorID");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Qrcode)
                .HasMaxLength(255)
                .HasColumnName("QRCode");
            entity.Property(e => e.TimeSlotId).HasColumnName("TimeSlotID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AppointmentStatus).WithMany(p => p.DonationAppointments)
                .HasForeignKey(d => d.AppointmentStatusId)
                .HasConstraintName("FK__DonationA__Appoi__03F0984C");

            entity.HasOne(d => d.DonationType).WithMany(p => p.DonationAppointments)
                .HasForeignKey(d => d.DonationTypeId)
                .HasConstraintName("FK__DonationA__Donat__04E4BC85");

            entity.HasOne(d => d.Donor).WithMany(p => p.DonationAppointments)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationA__Donor__05D8E0BE");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.DonationAppointments)
                .HasForeignKey(d => d.TimeSlotId)
                .HasConstraintName("FK__DonationA__TimeS__06CD04F7");
        });

        modelBuilder.Entity<DonationAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__Donation__DA397991713DA4BA");

            entity.ToTable("DonationAvailability");

            entity.Property(e => e.AvailabilityId).HasColumnName("AvailabilityID");
            entity.Property(e => e.AvailabilityName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<DonationRecord>(entity =>
        {
            entity.HasKey(e => e.DonationRecordId).HasName("PK__Donation__26D61356D0EE92FB");

            entity.ToTable("DonationRecord");

            entity.HasIndex(e => e.AppointmentId, "IX_DonationRecord_AppointmentID");

            entity.HasIndex(e => e.DonorId, "IX_DonationRecord_DonorID");

            entity.Property(e => e.DonationRecordId).HasColumnName("DonationRecordID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.BloodPressure).HasMaxLength(20);
            entity.Property(e => e.BloodTemperature).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DonationTypeId).HasColumnName("DonationTypeID");
            entity.Property(e => e.DonorId).HasColumnName("DonorID");
            entity.Property(e => e.DonorWeight).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ProcessingStaffId).HasColumnName("ProcessingStaffID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.VolumeExtracted).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.Appointment).WithMany(p => p.DonationRecords)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__Appoi__673F4B05");

            entity.HasOne(d => d.BloodTestResultNavigation).WithMany(p => p.DonationRecords)
                .HasForeignKey(d => d.BloodTestResult)
                .HasConstraintName("FK_DonationRecord_BloodTestResult");

            entity.HasOne(d => d.DonationType).WithMany(p => p.DonationRecords)
                .HasForeignKey(d => d.DonationTypeId)
                .HasConstraintName("FK__DonationR__Donat__6A1BB7B0");

            entity.HasOne(d => d.Donor).WithMany(p => p.DonationRecordDonors)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__Donor__68336F3E");

            entity.HasOne(d => d.ProcessingStaff).WithMany(p => p.DonationRecordProcessingStaffs)
                .HasForeignKey(d => d.ProcessingStaffId)
                .HasConstraintName("FK__DonationR__Proce__69279377");
        });

        modelBuilder.Entity<DonationType>(entity =>
        {
            entity.HasKey(e => e.DonationTypeId).HasName("PK__Donation__39DA5ED4397B58B6");

            entity.ToTable("DonationType");

            entity.Property(e => e.DonationTypeId).HasColumnName("DonationTypeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__Gender__4E24E81742354AD1");

            entity.ToTable("Gender");

            entity.Property(e => e.GenderId).HasColumnName("GenderID");
            entity.Property(e => e.GenderName).HasMaxLength(50);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E321EDA3C5D");

            entity.ToTable("Notification");

            entity.HasIndex(e => e.IsRead, "IX_Notification_IsRead");

            entity.HasIndex(e => e.RecipientId, "IX_Notification_RecipientID");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");
            entity.Property(e => e.RecipientId).HasColumnName("RecipientID");
            entity.Property(e => e.SentDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Subject).HasMaxLength(200);

            entity.HasOne(d => d.NotificationType).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__Notif__0C85DE4D");

            entity.HasOne(d => d.Recipient).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__Recip__0D7A0286");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.NotificationTypeId).HasName("PK__Notifica__299002A131834735");

            entity.ToTable("NotificationType");

            entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Occupation>(entity =>
        {
            entity.HasKey(e => e.OccupationId).HasName("PK__Occupati__8917118DD2D71214");

            entity.ToTable("Occupation");

            entity.Property(e => e.OccupationId).HasColumnName("OccupationID");
            entity.Property(e => e.OccupationName).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3AECB2A30F");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__TimeSlot__0A124A4F6565C918");

            entity.ToTable("TimeSlot");

            entity.Property(e => e.SlotId).HasColumnName("SlotID");
        });

        modelBuilder.Entity<Urgency>(entity =>
        {
            entity.HasKey(e => e.UrgencyId).HasName("PK__Urgency__7A92287A0B219FF9");

            entity.ToTable("Urgency");

            entity.Property(e => e.UrgencyId).HasColumnName("UrgencyID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UrgencyName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACB5F023D3");

            entity.ToTable("User");

            entity.HasIndex(e => e.BloodTypeId, "IX_User_BloodTypeID");

            entity.HasIndex(e => e.Email, "IX_User_Email");

            entity.HasIndex(e => e.RoleId, "IX_User_RoleID");

            entity.HasIndex(e => e.Username, "IX_User_Username");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E4ABBB6924").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DonationCount).HasDefaultValue(0);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.GenderId).HasColumnName("GenderID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NationalId)
                .HasMaxLength(20)
                .HasColumnName("NationalID");
            entity.Property(e => e.OccupationId).HasColumnName("OccupationID");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.StaffCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.BloodType).WithMany(p => p.Users)
                .HasForeignKey(d => d.BloodTypeId)
                .HasConstraintName("FK__User__BloodTypeI__0E6E26BF");

            entity.HasOne(d => d.DonationAvailabilityNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.DonationAvailability)
                .HasConstraintName("FK__User__DonationAv__0F624AF8");

            entity.HasOne(d => d.Gender).WithMany(p => p.Users)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK__User__GenderID__10566F31");

            entity.HasOne(d => d.Occupation).WithMany(p => p.Users)
                .HasForeignKey(d => d.OccupationId)
                .HasConstraintName("FK__User__Occupation__114A936A");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleID__123EB7A3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

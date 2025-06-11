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

    public virtual DbSet<DonationAvailability> DonationAvailabilities { get; set; }

    public virtual DbSet<DonationRecord> DonationRecords { get; set; }

    public virtual DbSet<DonationRegistration> DonationRegistrations { get; set; }

    public virtual DbSet<DonationSchedule> DonationSchedules { get; set; }

    public virtual DbSet<DonationType> DonationTypes { get; set; }

    public virtual DbSet<DonationValidation> DonationValidations { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Occupation> Occupations { get; set; }

    public virtual DbSet<RegistrationStatus> RegistrationStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<Urgency> Urgencies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=database.purintech.id.vn;user=sa;password=<Hu@nH0aH0n9>;Database=BloodDonationDB;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Article__9C6270C856B903A4");

            entity.ToTable("Article");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.ArticleCategoryId).HasColumnName("ArticleCategoryID");
            entity.Property(e => e.ArticleStatusId).HasColumnName("ArticleStatusID");
            entity.Property(e => e.AuthorUserId).HasColumnName("AuthorUserID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Picture).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.ArticleCategory).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticleCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Article_Category");

            entity.HasOne(d => d.ArticleStatus).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Article_Status");

            entity.HasOne(d => d.AuthorUser).WithMany(p => p.Articles)
                .HasForeignKey(d => d.AuthorUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Article_Author");
        });

        modelBuilder.Entity<ArticleCategory>(entity =>
        {
            entity.HasKey(e => e.ArticleCategoryId).HasName("PK__ArticleC__E0B60963DA213857");

            entity.ToTable("ArticleCategory");

            entity.HasIndex(e => e.CategoryName, "UQ_ArticleCategory_Name").IsUnique();

            entity.Property(e => e.ArticleCategoryId).HasColumnName("ArticleCategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<ArticleStatus>(entity =>
        {
            entity.HasKey(e => e.ArticleStatusId).HasName("PK__ArticleS__3F0E2D6BF9932617");

            entity.ToTable("ArticleStatus");

            entity.HasIndex(e => e.StatusName, "UQ_ArticleStatus_Name").IsUnique();

            entity.Property(e => e.ArticleStatusId).HasColumnName("ArticleStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BloodComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__BloodCom__D79CF02E08C0B55C");

            entity.ToTable("BloodComponent");

            entity.HasIndex(e => e.ComponentName, "UQ_BloodComponent_Name").IsUnique();

            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.ComponentName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BloodRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__BloodReq__33A8519A34EF2698");

            entity.ToTable("BloodRequest");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.BloodComponentId).HasColumnName("BloodComponentID");
            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Quantity).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.RequestStatusId).HasColumnName("RequestStatusID");
            entity.Property(e => e.RequestingStaffId).HasColumnName("RequestingStaffID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UrgencyId).HasColumnName("UrgencyID");

            entity.HasOne(d => d.BloodComponent).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.BloodComponentId)
                .HasConstraintName("FK_BloodRequest_Component");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodRequest_BloodType");

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.RequestStatusId)
                .HasConstraintName("FK_BloodRequest_Status");

            entity.HasOne(d => d.RequestingStaff).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.RequestingStaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodRequest_Staff");

            entity.HasOne(d => d.Urgency).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.UrgencyId)
                .HasConstraintName("FK_BloodRequest_Urgency");
        });

        modelBuilder.Entity<BloodRequestStatus>(entity =>
        {
            entity.HasKey(e => e.BloodRequestStatusId).HasName("PK__BloodReq__F73749E51AC96C55");

            entity.ToTable("BloodRequestStatus");

            entity.HasIndex(e => e.StatusName, "UQ_BloodRequestStatus_Name").IsUnique();

            entity.Property(e => e.BloodRequestStatusId).HasColumnName("BloodRequestStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BloodTestResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__BloodTes__97690228FF97CA20");

            entity.ToTable("BloodTestResult");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ResultName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId).HasName("PK__BloodTyp__B489BA431BE95202");

            entity.ToTable("BloodType");

            entity.HasIndex(e => e.BloodTypeName, "UQ_BloodType_Name").IsUnique();

            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.BloodTypeName).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.BloodUnitId).HasName("PK__BloodUni__AC1C2FAB6CA7BE7A");

            entity.ToTable("BloodUnit");

            entity.Property(e => e.BloodUnitId).HasColumnName("BloodUnitID");
            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.BloodUnitStatusId).HasColumnName("BloodUnitStatusID");
            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.DonationRecordId).HasColumnName("DonationRecordID");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.Volume).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodUnit_BloodType");

            entity.HasOne(d => d.BloodUnitStatus).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.BloodUnitStatusId)
                .HasConstraintName("FK_BloodUnit_Status");

            entity.HasOne(d => d.Component).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("FK_BloodUnit_Component");

            entity.HasOne(d => d.DonationRecord).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.DonationRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodUnit_DonationRecord");
        });

        modelBuilder.Entity<BloodUnitStatus>(entity =>
        {
            entity.HasKey(e => e.BloodUnitStatusId).HasName("PK__BloodUni__D4B59B311C3A5EAB");

            entity.ToTable("BloodUnitStatus");

            entity.HasIndex(e => e.StatusName, "UQ_BloodUnitStatus_Name").IsUnique();

            entity.Property(e => e.BloodUnitStatusId).HasColumnName("BloodUnitStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<DonationAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__Donation__DA397991555BA84B");

            entity.ToTable("DonationAvailability");

            entity.HasIndex(e => e.AvailabilityName, "UQ_DonationAvailability_Name").IsUnique();

            entity.Property(e => e.AvailabilityId).HasColumnName("AvailabilityID");
            entity.Property(e => e.AvailabilityName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<DonationRecord>(entity =>
        {
            entity.HasKey(e => e.DonationRecordId).HasName("PK__Donation__26D6135632AA38F6");

            entity.ToTable("DonationRecord");

            entity.HasIndex(e => e.RegistrationId, "UQ_DonationRecord_RegistrationID").IsUnique();

            entity.Property(e => e.DonationRecordId).HasColumnName("DonationRecordID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.DonationTypeId).HasColumnName("DonationTypeID");
            entity.Property(e => e.DonorBloodPressure).HasMaxLength(20);
            entity.Property(e => e.DonorTemperature).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.DonorWeight).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.VolumeDonated).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.BloodTestResultNavigation).WithMany(p => p.DonationRecords)
                .HasForeignKey(d => d.BloodTestResult)
                .HasConstraintName("FK_DonationRecord_BloodTestResult");

            entity.HasOne(d => d.DonationType).WithMany(p => p.DonationRecords)
                .HasForeignKey(d => d.DonationTypeId)
                .HasConstraintName("FK_DonationRecord_DonationType");

            entity.HasOne(d => d.Registration).WithOne(p => p.DonationRecord)
                .HasForeignKey<DonationRecord>(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationRecord_Registration");
        });

        modelBuilder.Entity<DonationRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Donation__6EF5883097CE0A91");

            entity.ToTable("DonationRegistration");

            entity.HasIndex(e => e.QrCodeUrl, "UQ_DonationRegistration_QrCodeUrl")
                .IsUnique()
                .HasFilter("([QrCodeUrl] IS NOT NULL)");

            entity.HasIndex(e => new { e.ScheduleId, e.DonorId }, "UQ_DonationRegistration_Schedule_Donor").IsUnique();

            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.DonorId).HasColumnName("DonorID");
            entity.Property(e => e.QrCodeUrl).HasMaxLength(255);
            entity.Property(e => e.RegistrationStatusId).HasColumnName("RegistrationStatusID");
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.TimeSlotId).HasColumnName("TimeSlotID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.Donor).WithMany(p => p.DonationRegistrations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationRegistration_Donor");

            entity.HasOne(d => d.RegistrationStatus).WithMany(p => p.DonationRegistrations)
                .HasForeignKey(d => d.RegistrationStatusId)
                .HasConstraintName("FK_DonationRegistration_RegistrationStatus");

            entity.HasOne(d => d.Schedule).WithMany(p => p.DonationRegistrations)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationRegistration_Schedule");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.DonationRegistrations)
                .HasForeignKey(d => d.TimeSlotId)
                .HasConstraintName("FK_DonationRegistration_TimeSlot");
        });

        modelBuilder.Entity<DonationSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Donation__9C8A5B691113732F");

            entity.ToTable("DonationSchedule");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<DonationType>(entity =>
        {
            entity.HasKey(e => e.DonationTypeId).HasName("PK__Donation__39DA5ED4246E11EC");

            entity.ToTable("DonationType");

            entity.HasIndex(e => e.TypeName, "UQ_DonationType_Name").IsUnique();

            entity.Property(e => e.DonationTypeId).HasColumnName("DonationTypeID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<DonationValidation>(entity =>
        {
            entity.HasKey(e => e.ValidationId).HasName("PK__Donation__FA0B50E573AB1224");

            entity.ToTable("DonationValidation");

            entity.HasIndex(e => new { e.UserId, e.DonationRecordId }, "UQ_Validation_User_Record").IsUnique();

            entity.Property(e => e.ValidationId).HasColumnName("ValidationID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.DonationRecordId).HasColumnName("DonationRecordID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.DonationRecord).WithMany(p => p.DonationValidations)
                .HasForeignKey(d => d.DonationRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationValidation_DonationRecord");

            entity.HasOne(d => d.User).WithMany(p => p.DonationValidations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationValidation_User");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__Gender__4E24E8174B05E886");

            entity.ToTable("Gender");

            entity.Property(e => e.GenderId).HasColumnName("GenderID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.GenderName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32D46227A7");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");
            entity.Property(e => e.RecipientId).HasColumnName("RecipientID");
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.NotificationType).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_Type");

            entity.HasOne(d => d.Recipient).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_Recipient");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.NotificationTypeId).HasName("PK__Notifica__299002A1A3ACB71E");

            entity.ToTable("NotificationType");

            entity.HasIndex(e => e.TypeName, "UQ_NotificationType_Name").IsUnique();

            entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Occupation>(entity =>
        {
            entity.HasKey(e => e.OccupationId).HasName("PK__Occupati__8917118D81C87CF2");

            entity.ToTable("Occupation");

            entity.Property(e => e.OccupationId).HasColumnName("OccupationID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.OccupationName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<RegistrationStatus>(entity =>
        {
            entity.HasKey(e => e.RegistrationStatusId).HasName("PK__Registra__17166B45D3376A20");

            entity.ToTable("RegistrationStatus");

            entity.HasIndex(e => e.RegistrationStatusName, "UQ_RegistrationStatus_Name").IsUnique();

            entity.Property(e => e.RegistrationStatusId).HasColumnName("RegistrationStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RegistrationStatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A295B582F");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ_Role_Name").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.TimeSlotId).HasName("PK__TimeSlot__41CC1F52B6CB9425");

            entity.ToTable("TimeSlot");

            entity.Property(e => e.TimeSlotId).HasColumnName("TimeSlotID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.TimeSlotName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Urgency>(entity =>
        {
            entity.HasKey(e => e.UrgencyId).HasName("PK__Urgency__7A92287AD00C34BC");

            entity.ToTable("Urgency");

            entity.HasIndex(e => e.UrgencyName, "UQ_Urgency_Name").IsUnique();

            entity.Property(e => e.UrgencyId).HasColumnName("UrgencyID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UrgencyName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACAFEFE9AC");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ_User_Email")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL)");

            entity.HasIndex(e => e.NationalId, "UQ_User_NationalID")
                .IsUnique()
                .HasFilter("([NationalID] IS NOT NULL)");

            entity.HasIndex(e => e.PhoneNumber, "UQ_User_PhoneNumber")
                .IsUnique()
                .HasFilter("([PhoneNumber] IS NOT NULL)");

            entity.HasIndex(e => e.StaffCode, "UQ_User_StaffCode")
                .IsUnique()
                .HasFilter("([StaffCode] IS NOT NULL)");

            entity.HasIndex(e => e.Username, "UQ_User_Username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodTypeId).HasColumnName("BloodTypeID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DonationAvailabilityId)
                .HasDefaultValue(1)
                .HasColumnName("DonationAvailabilityID");
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
            entity.Property(e => e.PhoneNumber).HasMaxLength(25);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.StaffCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.BloodType).WithMany(p => p.Users)
                .HasForeignKey(d => d.BloodTypeId)
                .HasConstraintName("FK_User_BloodType");

            entity.HasOne(d => d.DonationAvailability).WithMany(p => p.Users)
                .HasForeignKey(d => d.DonationAvailabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_DonationAvailability");

            entity.HasOne(d => d.Gender).WithMany(p => p.Users)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_User_Gender");

            entity.HasOne(d => d.Occupation).WithMany(p => p.Users)
                .HasForeignKey(d => d.OccupationId)
                .HasConstraintName("FK_User_Occupation");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

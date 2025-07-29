using BE_Homnayangi.Ultils.EmailServices;
using BloodDonationSupportSystem.Utils;
using BuisinessLayer.Utils.EmailConfiguration;
using BusinessLayer.IService;
using BusinessLayer.QuartzJobs.Job;
using BusinessLayer.QuartzJobs.Schedulers;
using BusinessLayer.Service;
using BusinessLayer.Utils;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ====================== CONFIGURATION ====================== //
// Add User Secrets for development environment
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));

// Load and validate EmailConfiguration
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

// Validate EmailConfiguration
if (emailConfig == null)
{
    throw new InvalidOperationException("EmailConfiguration is not properly configured in appsettings.json or User Secrets.");
}

if (string.IsNullOrEmpty(emailConfig.From) ||
    string.IsNullOrEmpty(emailConfig.SmtpServer) ||
    string.IsNullOrEmpty(emailConfig.Username) || // Changed from UserName to Username
    string.IsNullOrEmpty(emailConfig.Password))
{
    throw new InvalidOperationException("EmailConfiguration is missing required fields (From, SmtpServer, Username, Password).");
}

Console.WriteLine($"Email configuration loaded successfully - From: {emailConfig.From}, SMTP: {emailConfig.SmtpServer}:{emailConfig.Port}");

builder.Services.AddSingleton(emailConfig);

builder.Services.Configure<CertificateSettings>(
    builder.Configuration.GetSection("CertificateSettings"));

// ====================== QUARTZ CONFIGURATION ====================== //
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // --- Job tự động tạo lịch hiến máu ---
    var autoScheduleJobKey = new JobKey("AutoScheduleCreationJob");
    q.AddJob<AutoScheduleCreationJob>(opts => opts.WithIdentity(autoScheduleJobKey));

    // Chạy job này vào 1 giờ sáng mỗi ngày
    q.AddTrigger(opts => opts
        .ForJob(autoScheduleJobKey)
        .WithIdentity("AutoScheduleCreationJob-trigger")
        .WithCronSchedule("0 0 1 * * ?") // Cú pháp Cron: giây phút giờ ngày tháng WDAY
        .WithDescription("Trigger to run auto schedule creation job daily at 1 AM")
    );
});

builder.Services.AddTransient<NotifQuartzScheduler>();
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ====================== CONTROLLERS & API BEHAVIOR ====================== //
builder.Services.AddControllers();
builder.Services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = (errorContext) =>
    {
        var errorMessages = errorContext.ModelState.Values
            .SelectMany(e => e.Errors.Select(m => m.ErrorMessage))
            .ToList();

        var result = new
        {
            status = "failed",
            msg = errorMessages
        };
        return new BadRequestObjectResult(result);
    };
});

// ====================== DATABASE ====================== //
builder.Services.AddDbContext<BloodDonationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BloodDonationDB")));

// ====================== AUTHENTICATION ====================== //
string? secretKey = builder.Configuration["AppSetting:SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("AppSetting:SecretKey is not configured.");
}
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            RoleClaimType = "RoleID", // hoặc "roles", tùy vào key trong JWT
        };
    });

// ====================== CORS ====================== //
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ====================== SWAGGER ====================== //
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Set the comments path for the Swagger JSON and UI
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add JWT Authentication support in Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                 {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ====================== AUTOMAPPER ====================== //
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// ====================== DEPENDENCY INJECTION ====================== //
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Services
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IDonationRegistrationServices, DonationRegistrationService>();
builder.Services.AddScoped<ITimeSlotServices, TimeSlotServices>();
builder.Services.AddScoped<IDonationRecordService, DonationRecordService>();
builder.Services.AddScoped<IDonationScheduleService, DonationScheduleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBloodUnitService, BloodUnitService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
builder.Services.AddScoped<IHospitalService, HospitalService>();
builder.Services.AddScoped<IBloodCompatibilityService, BloodCompatibilityService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IEmergencyBloodEmailService, EmergencyBloodEmailService>();

builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();

builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();


// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDonationRegistrationRepository, DonationRegistrationRepository>();
builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
builder.Services.AddScoped<IDonationRecordRepository, DonationRecordRepository>();
builder.Services.AddScoped<IDonationScheduleRepository, DonationScheduleRepository>();
builder.Services.AddScoped<IBloodUnitRepository, BloodUnitRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();
builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
builder.Services.AddScoped<IBloodCompatibilityRepository, BloodCompatibilityRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

// Generic Repositories
builder.Services.AddScoped<IGenericRepository<Gender>, GenericRepository<Gender>>();
builder.Services.AddScoped<IGenericRepository<BloodType>, GenericRepository<BloodType>>();
builder.Services.AddScoped<IGenericRepository<BloodComponent>, GenericRepository<BloodComponent>>();
builder.Services.AddScoped<IGenericRepository<BloodUnitStatus>, GenericRepository<BloodUnitStatus>>();
builder.Services.AddScoped<IGenericRepository<DonationAvailability>, GenericRepository<DonationAvailability>>();
builder.Services.AddScoped<IGenericRepository<DonationType>, GenericRepository<DonationType>>();
builder.Services.AddScoped<IGenericRepository<NotificationType>, GenericRepository<NotificationType>>();
builder.Services.AddScoped<IGenericRepository<Occupation>, GenericRepository<Occupation>>();
builder.Services.AddScoped<IGenericRepository<Role>, GenericRepository<Role>>();
builder.Services.AddScoped<IGenericRepository<Urgency>, GenericRepository<Urgency>>();
builder.Services.AddScoped<IGenericRepository<BloodRequestStatus>, GenericRepository<BloodRequestStatus>>();
builder.Services.AddScoped<IGenericRepository<RegistrationStatus>, GenericRepository<RegistrationStatus>>();
builder.Services.AddScoped<IGenericRepository<ArticleCategory>, GenericRepository<ArticleCategory>>();
builder.Services.AddScoped<IGenericRepository<ArticleStatus>, GenericRepository<ArticleStatus>>();
builder.Services.AddScoped<IGenericRepository<BloodTestResult>, GenericRepository<BloodTestResult>>();

// ====================== BUILD APPLICATION ====================== //
var app = builder.Build();

// ====================== MIDDLEWARE PIPELINE ====================== //
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Fix CORS - only use one policy that exists
app.UseCors("AllowCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using BE_Homnayangi.Ultils.EmailServices;
using BloodDonationSupportSystem.Utils;
using BuisinessLayer.Utils.EmailConfiguration;
using BusinessLayer.IService;
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
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));
//new
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.Configure<CertificateSettings>(
    builder.Configuration.GetSection("CertificateSettings"));
builder.Services.AddQuartz();
builder.Services.AddTransient<NotifQuartzScheduler>();
builder.Services.AddQuartzHostedService();

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
builder.Services.AddSwaggerGen();

// ====================== AUTOMAPPER ====================== //
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// ====================== DEPENDENCY INJECTION ====================== //
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Services
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IDonationRegistrationServices, DonationRegistrationService>();
builder.Services.AddScoped<ITimeSlotServices, TimeSlotServices>();
builder.Services.AddScoped<IDonationRecordService, DonationRecordService>();

builder.Services.AddScoped<IDonationScheduleRepository, DonationScheduleRepository>();
builder.Services.AddScoped<IDonationScheduleService, DonationScheduleService>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBloodUnitService, BloodUnitService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService > ();
builder.Services.AddScoped<INotificationService, NotificationService>();
// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDonationRegistrationRepository, DonationRegistrationRepository>();
builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
builder.Services.AddScoped<IDonationRecordRepository, DonationRecordRepository>();
builder.Services.AddScoped<IDonationScheduleRepository, DonationScheduleRepository>();
builder.Services.AddScoped<IBloodUnitRepository, BloodUnitRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

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

}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");
app.UseCors("AllowCors");
app.UseCors("AllowReact");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using BE_Homnayangi.Ultils.EmailServices;
using BloodDonationSupportSystem.Utils;
using BuisinessLayer.Utils.EmailConfiguration;
using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));
//new
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddControllers();
builder.Services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = (errorContext) =>
    {
        var errorMessages = errorContext.ModelState.Values
            .SelectMany(e => e.Errors
            .Select(m => m.ErrorMessage))
            .ToList();

        var result = new
        {
            status = "failed",
            msg = errorMessages  
        };
        return new BadRequestObjectResult(result);
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        builder => builder.WithOrigins("http://localhost:3000") // Adjust the origin as needed
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});
string? secretKey = builder.Configuration["AppSetting:SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    // Handle the case where the secret key is missing,
    // perhaps throw an exception or log an error,
    // as Encoding.UTF8.GetBytes will throw if secretKey is null.
    throw new InvalidOperationException("AppSetting:SecretKey is not configured.");
}
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        };
        // Configure JWT Bearer options (issuer, audience, key, etc.)
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BloodDonationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BloodDonationDB")));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IDonationRegistrationRepository, DonationRegistrationRepository>();
builder.Services.AddScoped<IDonationRegistrationServices, DonationRegistrationService>();
builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();  
builder.Services.AddScoped<ITimeSlotServices, TimeSlotServices>();
builder.Services.AddScoped<IDonationRecordRepository, DonationRecordRepository>();
builder.Services.AddScoped<IDonationRecordService, DonationRecordService>();
builder.Services.AddScoped<IDonationScheduleRepository, DonationScheduleRepository>();
builder.Services.AddScoped<IDonationScheduleService, DonationScheduleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBloodUnitRepository, BloodUnitRepository>();
builder.Services.AddScoped<IBloodUnitService, BloodUnitService>();
builder.Services.AddScoped<ILookupService, LookupService>();
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
           policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // Common React dev servers
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials(); // Allows cookies/credentials to be sent
       });
   });
var app = builder.Build();


// Configure the HTTP request pipeline.
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

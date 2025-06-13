using BloodDonationSupportSystem.Utils;
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
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

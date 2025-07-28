using AutoMapper;
using Azure.Core;
using BloodDonationSupportSystem.Utils;
using BuisinessLayer.Utils.EmailConfiguration;
using BusinessLayer.IService;
using BusinessLayer.Utils;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSetting _appSetting;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly IHospitalService _hospitalService;

        public UserServices(IUserRepository userRepository, IMapper mapper, IOptionsMonitor<AppSetting> options, IHttpContextAccessor httpContextAccessor, IEmailService emailService, IHospitalService hospitalService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appSetting = options.CurrentValue;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _emailService = emailService;
            _hospitalService = hospitalService;
        }

        private DateTime GetVietnamTime()
        {
            try
            {
                // Thử tìm múi giờ cho Windows
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            }
            catch (Exception)
            {
                return DateTime.UtcNow.AddHours(7);
            }
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty");
            }

            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            if (roleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roleId), "Role ID must be greater than zero");
            }

            return await _userRepository.GetByRoleIdAsync(roleId);
        }

        public async Task<IEnumerable<User>> GetUsersByBloodTypeAsync(int bloodTypeId)
        {
            if (bloodTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bloodTypeId), "Blood Type ID must be greater than zero");
            }

            return await _userRepository.GetByBloodTypeIdAsync(bloodTypeId);
        }

        public async Task<IEnumerable<User>> GetEligibleDonorsAsync()
        {
            return await _userRepository.GetEligibleDonorsAsync();
        }

        public async Task RegisterDonorAsync(RegisterDTO donor)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(donor.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }
                User EntityUser = _mapper.Map<User>(donor);

                EntityUser.UserImage = null;

                EntityUser.PasswordHash = EncryptPassword(donor.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 3;
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();

                SendWelcomeEmail(donor.Email, EntityUser.FullName ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                throw; 
            }
        }

        public async Task RegisterStaffAsync(StaffRegisterDTO staff)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(staff.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }
                User EntityUser = _mapper.Map<User>(staff);

                EntityUser.UserImage = null;

                EntityUser.PasswordHash = EncryptPassword("staff123");
                EntityUser.IsActive = true;
                EntityUser.RoleId = 2; 
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();

                SendWelcomeEmail(staff.Email, EntityUser.FullName ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding staff: {ex.Message}");
                throw;
            }
        }

        public async Task RegisterAdminAsync(StaffRegisterDTO admin)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(admin.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }
                User EntityUser = _mapper.Map<User>(admin);

                EntityUser.UserImage = null;

                EntityUser.PasswordHash = EncryptPassword("staff123");
                EntityUser.IsActive = true;
                EntityUser.RoleId = 1; 
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();

                SendWelcomeEmail(admin.Email, EntityUser.FullName ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding admin: {ex.Message}");
                throw; 
            }
        }
        public async Task RegisterHospitalAsync(HospitalRegisterDTO hospital)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(hospital.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }
                User EntityUser = _mapper.Map<User>(hospital);
                EntityUser.PasswordHash = EncryptPassword("hospital123");
                EntityUser.IsActive = true;
                EntityUser.RoleId = 4;
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();
                var hospitalName = _hospitalService.GetHospitalByIdAsync(hospital.HospitalId).Result.HospitalName;
                SendWelcomeEmail(hospital.Email, hospitalName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding hospital: {ex.Message}");
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }
            var existingUser = await _userRepository.GetByPhoneNumberAsync(user.PhoneNumber);
            if (existingUser != null && existingUser.UserId != user.UserId)
            {
                throw new InvalidOperationException("Phone number already exists for another user");
            }
            existingUser = await _userRepository.GetByNationalIdAsync(user.NationalId);
            if (existingUser != null && existingUser.UserId != user.UserId)
            {
                throw new InvalidOperationException("National ID already exists for another user");
            }
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateDonorAsync(int donorId, DonorDTO donor)
        {
            if (donor == null || donorId <= 0)
            {
                throw new ArgumentNullException(nameof(donor), "Donor data cannot be null or empty");
            }

            var existingUser = await _userRepository.GetByIdAsync(donorId);

            if (existingUser == null)
            {
                throw new KeyNotFoundException($"Donor with ID {donorId} not found");
            }

            _mapper.Map(donor, existingUser);

            // Set updated timestamp
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _userRepository.UpdateAsync(existingUser);
            await _userRepository.SaveChangesAsync();

            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            // Soft delete - get the user first and set IsDeleted flag
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            bool result = await _userRepository.UpdateDonationInfoAsync(userId, donationDate);
            if (result)
            {
                await _userRepository.SaveChangesAsync();
            }
            return result;
        }

        public async Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabililtyId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            bool result = await _userRepository.UpdateUserDonationAvailabilityAsync(userId, donationAvailabililtyId);
            if (result)
            {
                await _userRepository.SaveChangesAsync();
            }
            return result;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<byte[]> GetUserImageAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            return user?.UserImage;
        }

        public async Task<bool> DeleteUserImageAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.UserImage = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<string> GenerateToken(LoginDTO login)
        {
            try
            {
                // First try to get user directly by email for better performance
                var user = await _userRepository.GetByEmailAsync(login.Email);

                // If no user found or password doesn't match
                if (user == null || user.PasswordHash != EncryptPassword(login.PasswordHash))
                {
                    Console.WriteLine($"Login failed: User with email {login.Email} not found or password doesn't match");
                    return null;
                }

                var jwtTokenHandler = new JwtSecurityTokenHandler();

                // Check if SecretKey is available
                if (string.IsNullOrEmpty(_appSetting.SecretKey))
                {
                    Console.WriteLine("Login failed: SecretKey is not configured");
                    return null;
                }

                var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim("UserID", user.UserId.ToString()),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim("PhoneNumber", user.PhoneNumber ?? ""),
                        new Claim("FullName", user.FullName ?? ""),
                        new Claim("DateOfBirth", user.DateOfBirth?.ToString("yyyy-MM-dd") ?? ""),
                        new Claim("RoleID", user.RoleId.ToString()),
                        new Claim("NationalID", user.NationalId ?? ""),
                        new Claim("Address", user.Address ?? ""),
                        new Claim("GenderID", user.GenderId?.ToString() ?? ""),
                        new Claim("OccupationID", user.OccupationId?.ToString() ?? ""),
                        new Claim("BloodTypeID", user.BloodTypeId?.ToString() ?? ""),
                        new Claim("LastDonationDate", user.LastDonationDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                        new Claim("NextEligibleDonationDate", user.NextEligibleDonationDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                        new Claim("DonationAvailabilityID", user.DonationAvailabilityId.ToString()),
                        new Claim("IsActive", user.IsActive.ToString())
                    }),

                    Expires = DateTime.UtcNow.AddMinutes(180),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
                };

                var principal = new ClaimsPrincipal(tokenDescription.Subject);
                _httpContextAccessor.HttpContext.User = principal;

                var token = jwtTokenHandler.CreateToken(tokenDescription);
                return jwtTokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating token: {ex.Message}");
                throw;
            }
        }

        public string EncryptPassword(string plainText)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public string DecryptPassword(string cipherText)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public async Task<string> ValidateGoogleToken(TokenRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "1038271412034-f887nt2v6kln6nb09e20pvjgfo1o7jn0.apps.googleusercontent.com" }
                });
                string email = payload.Email;
                var user= await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    RegisterDTO googleDTO = new RegisterDTO
                    {
                        Email = payload.Email,
                        PasswordHash = EncryptPassword(Guid.NewGuid().ToString()),
                    };
                    await RegisterDonorAsync(googleDTO);
                    await _userRepository.SaveChangesAsync();
                    user = await _userRepository.GetByEmailAsync(email); 
                    LoginDTO userLogin = _mapper.Map<LoginDTO>(user);
                    userLogin.PasswordHash = DecryptPassword(user.PasswordHash);
                    return await GenerateToken(userLogin);
                }
                else
                {
                    LoginDTO userLogin = _mapper.Map<LoginDTO>(user);
                    userLogin.PasswordHash = DecryptPassword(user.PasswordHash);
                    return await GenerateToken(userLogin);
                }
            }
            catch (InvalidJwtException ex)
            {
                Console.WriteLine($"Error generating token: {ex.Message}");
                throw;
            }
            return null;
        }

        public void SendMail(string mailSubject, string mailBody, string receiver)
        {
            var message = new Message(
                 to: new string[] { receiver },
                 subject: mailSubject,
                 content: mailBody);

            _emailService.SendEmail(message);
        }

        public void SendWelcomeEmail(string userEmail, string fullName = "")
        {
            try
            {
                // Add debug logging
                Console.WriteLine($"=== ATTEMPTING TO SEND WELCOME EMAIL ===");
                Console.WriteLine($"Email service is null: {_emailService == null}");
                Console.WriteLine($"Target email: {userEmail}");
                Console.WriteLine($"Full name: {fullName}");

                var subject = "Chào mừng bạn đến với Hệ thống Hỗ trợ Hiến máu!";
                var htmlBody = GenerateWelcomeEmailTemplate(fullName, userEmail);

                var message = new Message(
                    to: new string[] { userEmail },
                    subject: subject,
                    content: htmlBody);

                Console.WriteLine($"Created email message successfully");

                _emailService.SendEmail(message);

                Console.WriteLine($"=== EMAIL SENT SUCCESSFULLY to {userEmail} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR SENDING WELCOME EMAIL ===");
                Console.WriteLine($"Target email: {userEmail}");
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"=== END ERROR LOG ===");

                // Don't throw - email failure shouldn't break registration
            }
        }

        private string GenerateWelcomeEmailTemplate(string fullName, string userEmail)
        {
            var displayName = !string.IsNullOrEmpty(fullName) ? fullName : userEmail;
            var currentDate = GetVietnamTime().ToString("dd/MM/yyyy HH:mm");

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='vi'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <title>Chào mừng</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; background-color: #f4f4f4; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; }");
            sb.AppendLine("        .header { background-color: #B22222; color: white; padding: 20px; text-align: center; }");
            sb.AppendLine("        .content { padding: 20px 0; }");
            sb.AppendLine("        .highlight { background-color: #f8f9fa; padding: 15px; border-left: 4px solid #B22222; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>Chào mừng đến với Blood Donation Support System!</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Xin chào {displayName}!</h2>");
            sb.AppendLine("            <p>Cảm ơn bạn đã đăng ký tài khoản tại Hệ thống Hỗ trợ Hiến máu.</p>");
            sb.AppendLine("            <div class='highlight'>");
            sb.AppendLine("                <h3>Tại đây bạn có thể:</h3>");
            sb.AppendLine("                <ul>");
            sb.AppendLine("                    <li>Đăng ký lịch hiến máu tình nguyện</li>");
            sb.AppendLine("                    <li>Theo dõi lịch sử hiến máu</li>");
            sb.AppendLine("                    <li>Nhận giấy chứng nhận hiến máu</li>");
            sb.AppendLine("                </ul>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <p><strong>Thông tin tài khoản:</strong></p>");
            sb.AppendLine("            <ul>");
            sb.AppendLine($"                <li>Email: {userEmail}</li>");
            sb.AppendLine($"                <li>Ngày đăng ký: {currentDate}</li>");
            sb.AppendLine("            </ul>");
            sb.AppendLine("            <p style='text-align: center;'><em>Hiến máu cứu người - Một nghĩa cử cao đẹp</em></p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public void SendDonationRegistrationThankYouEmail(string userEmail, string fullName, DonationRegistrationEmailInfoDTO registrationInfo)
        {
            try
            {
                Console.WriteLine($"=== ATTEMPTING TO SEND DONATION REGISTRATION THANK YOU EMAIL ===");
                Console.WriteLine($"Email service is null: {_emailService == null}");
                Console.WriteLine($"Target email: {userEmail}");
                Console.WriteLine($"Full name: {fullName}");
                Console.WriteLine($"Registration code: {registrationInfo.RegistrationCode}");

                var subject = "Cảm ơn bạn đã đăng ký hiến máu tình nguyện!";
                var htmlBody = GenerateDonationRegistrationThankYouEmailTemplate(fullName, userEmail, registrationInfo);

                var message = new Message(
                    to: new string[] { userEmail },
                    subject: subject,
                    content: htmlBody);

                Console.WriteLine($"Created donation registration email message successfully");

                _emailService.SendEmail(message);

                Console.WriteLine($"=== DONATION REGISTRATION EMAIL SENT SUCCESSFULLY to {userEmail} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR SENDING DONATION REGISTRATION THANK YOU EMAIL ===");
                Console.WriteLine($"Target email: {userEmail}");
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"=== END ERROR LOG ===");
            }
        }

        private string GenerateDonationRegistrationThankYouEmailTemplate(string fullName, string userEmail, DonationRegistrationEmailInfoDTO registrationInfo)
        {
            var displayName = !string.IsNullOrEmpty(fullName) ? fullName : registrationInfo.DonorName ?? userEmail;
            var currentDate = GetVietnamTime().ToString("dd/MM/yyyy HH:mm");

            // Tạo đối tượng CultureInfo cho tiếng Việt
            var vietnameseCulture = new CultureInfo("vi-VN");

            // Định dạng ngày hiến máu sang tiếng Việt
            var scheduleDateVietnamese = registrationInfo.ScheduleDate.ToString("dddd, 'ngày' dd/MM/yyyy", vietnameseCulture);


            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='vi'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <title>Xác nhận đăng ký hiến máu</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }");
            sb.AppendLine("        .header { background-color: #B22222; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }");
            sb.AppendLine("        .content { padding: 20px 0; }");
            sb.AppendLine("        .highlight { background-color: #f8f9fa; padding: 15px; border-left: 4px solid #B22222; margin: 15px 0; border-radius: 4px; }");
            sb.AppendLine("        .registration-info { background-color: #fff3cd; padding: 15px; border: 1px solid #ffeaa7; border-radius: 4px; margin: 15px 0; }");
            sb.AppendLine("        .schedule-info { background-color: #d1ecf1; padding: 15px; border: 1px solid #bee5eb; border-radius: 4px; margin: 15px 0; }");
            sb.AppendLine("        .important { color: #B22222; font-weight: bold; }");
            sb.AppendLine("        .footer { text-align: center; margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 10px 0; }");
            sb.AppendLine("        td { padding: 8px; border-bottom: 1px solid #eee; }");
            sb.AppendLine("        .label { font-weight: bold; width: 40%; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>🩸 Xác nhận đăng ký hiến máu thành công!</h1>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Kính chào {displayName}!</h2>");
            sb.AppendLine("            <p>Cảm ơn bạn đã đăng ký hiến máu tình nguyện tại Hệ thống Hỗ trợ Hiến máu. Đây là một hành động vô cùng ý nghĩa và cao đẹp!</p>");

            sb.AppendLine("            <div class='registration-info'>");
            sb.AppendLine("                <h3>📋 Thông tin đăng ký</h3>");
            sb.AppendLine("                <table>");
            sb.AppendLine($"                    <tr><td class='label'>Mã đăng ký:</td><td class='important'>{registrationInfo.RegistrationCode ?? registrationInfo.RegistrationId.ToString()}</td></tr>");
            sb.AppendLine($"                    <tr><td class='label'>Ngày đăng ký:</td><td>{registrationInfo.RegistrationDate.ToString("dd/MM/yyyy")}</td></tr>");
            sb.AppendLine($"                    <tr><td class='label'>Họ tên:</td><td>{registrationInfo.DonorName ?? displayName}</td></tr>");
            sb.AppendLine($"                    <tr><td class='label'>Email:</td><td>{registrationInfo.DonorEmail ?? userEmail}</td></tr>");

            if (!string.IsNullOrEmpty(registrationInfo.DonorPhone))
            {
                sb.AppendLine($"                    <tr><td class='label'>Số điện thoại:</td><td>{registrationInfo.DonorPhone}</td></tr>");
            }

            if (!string.IsNullOrEmpty(registrationInfo.BloodType))
            {
                sb.AppendLine($"                    <tr><td class='label'>Nhóm máu:</td><td class='important'>{registrationInfo.BloodType}</td></tr>");
            }

            sb.AppendLine("                </table>");
            sb.AppendLine("            </div>");

            sb.AppendLine("            <div class='schedule-info'>");
            sb.AppendLine("                <h3>📅 Thông tin lịch hiến máu</h3>");
            sb.AppendLine("                <table>");
            // SỬ DỤNG CHUỖI ĐÃ ĐỊNH DẠNG TIẾNG VIỆT
            sb.AppendLine($"                    <tr><td class='label'>Ngày hiến máu:</td><td class='important'>{scheduleDateVietnamese}</td></tr>");


            if (!string.IsNullOrEmpty(registrationInfo.StartTime) && !string.IsNullOrEmpty(registrationInfo.EndTime))
            {
                sb.AppendLine($"                    <tr><td class='label'>Thời gian:</td><td>{registrationInfo.StartTime} - {registrationInfo.EndTime}</td></tr>");
            }

            if (!string.IsNullOrEmpty(registrationInfo.ScheduleLocation))
            {
                sb.AppendLine($"                    <tr><td class='label'>Địa điểm:</td><td>{registrationInfo.ScheduleLocation}</td></tr>");
            }

            if (!string.IsNullOrEmpty(registrationInfo.HospitalName))
            {
                sb.AppendLine($"                    <tr><td class='label'>Bệnh viện:</td><td>{registrationInfo.HospitalName}</td></tr>");
            }

            if (!string.IsNullOrEmpty(registrationInfo.HospitalAddress))
            {
                sb.AppendLine($"                    <tr><td class='label'>Địa chỉ bệnh viện:</td><td>{registrationInfo.HospitalAddress}</td></tr>");
            }

            sb.AppendLine("                </table>");
            sb.AppendLine("            </div>");

            sb.AppendLine("            <div class='highlight'>");
            sb.AppendLine("                <h3>⚠️ Lưu ý quan trọng</h3>");
            sb.AppendLine("                <ul>");
            sb.AppendLine("                    <li>Vui lòng có mặt <strong>đúng giờ</strong> theo lịch đã đăng ký</li>");
            sb.AppendLine("                    <li>Mang theo <strong>CCCD/CMND</strong> để xác nhận danh tính</li>");
            sb.AppendLine("                    <li>Không uống rượu bia trong 24h trước khi hiến máu</li>");
            sb.AppendLine("                    <li>Ăn no trước khi hiến máu 3-4 tiếng và uống đủ nước</li>");
            sb.AppendLine("                    <li>Ngủ đủ giấc và không căng thẳng</li>");
            sb.AppendLine("                    <li>Nếu có thay đổi, vui lòng liên hệ trước ít nhất 1 ngày</li>");
            sb.AppendLine("                </ul>");
            sb.AppendLine("            </div>");

            sb.AppendLine("            <div class='highlight'>");
            sb.AppendLine("                <h3>📞 Liên hệ hỗ trợ</h3>");
            sb.AppendLine("                <p>Nếu bạn có bất kỳ thắc mắc nào hoặc cần thay đổi lịch hẹn, vui lòng liên hệ:</p>");
            sb.AppendLine("                <ul>");
            sb.AppendLine("                    <li>Email: giotmaunghiatinh@gmail.com</li>");
            sb.AppendLine("                </ul>");
            sb.AppendLine("            </div>");

            sb.AppendLine("            <div class='footer'>");
            sb.AppendLine("                <p><em style='color: #B22222; font-size: 18px;'>\"Hiến máu cứu người - Một nghĩa cử cao đẹp\"</em></p>");
            sb.AppendLine("                <p>Cảm ơn bạn đã đồng hành cùng chúng tôi trong công tác hiến máu nhân đạo!</p>");
            sb.AppendLine($"                <p style='font-size: 12px; color: #999;'>Email được gửi lúc: {currentDate}</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)

                return false;

            // 1. Tạo một token ngẫu nhiên và an toàn
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // 2. Cập nhật   token và thời gian hết hạn cho user
            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // 3. Gửi email chứa link reset
            // **QUAN TRỌNG**: Link này phải trỏ đến trang reset password trên Frontend của bạn
            var resetLink = $"http://localhost:3000/reset-password?token={token}";

            var subject = "Yêu cầu đặt lại mật khẩu";
            var body = GeneratePasswordResetEmailTemplate(user.FullName, resetLink);

            SendMail(subject, body, user.Email);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userRepository.GetByPasswordResetToken(token);

            // Kiểm tra xem token có hợp lệ và còn hạn không
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return false; // Invalid token or token expired
            }

            // Cập nhật mật khẩu mới (nhớ mã hóa)
            user.PasswordHash = EncryptPassword(newPassword);

            user.PasswordResetToken = null; // Clear the token after successful reset
            user.ResetTokenExpires = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        private string GeneratePasswordResetEmailTemplate(string fullName, string resetLink)
        {
            var emailTemplate = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Yêu cầu Đặt lại Mật khẩu</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse; margin-top: 30px; margin-bottom: 30px; border: 1px solid #cccccc;'>
        <tr>
            <td align='center' bgcolor='#B22222' style='padding: 30px 0; color: #ffffff; font-size: 28px; font-weight: bold;'>
                Hệ thống Hỗ trợ Hiến máu
            </td>
        </tr>
        <tr>
            <td bgcolor='#ffffff' style='padding: 40px 30px;'>
                <h1 style='font-size: 24px; color: #333333;'>Yêu cầu Đặt lại Mật khẩu</h1>
                <p style='margin: 20px 0; font-size: 16px; line-height: 1.5; color: #555555;'>
                    Xin chào {fullName},
                </p>
                <p style='margin: 20px 0; font-size: 16px; line-height: 1.5; color: #555555;'>
                    Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng nhấp vào nút bên dưới để tiến hành.
                </p>
                <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                    <tr>
                        <td align='center' style='padding: 20px 0;'>
                            <a href='{resetLink}' style='background-color: #dc3545; color: #ffffff; padding: 15px 25px; text-decoration: none; border-radius: 5px; font-size: 18px; display: inline-block;'>
                                Đặt lại Mật khẩu
                            </a>
                        </td>
                    </tr>
                </table>
                <p style='margin: 20px 0; font-size: 16px; line-height: 1.5; color: #555555;'>
                    Đường link này sẽ hết hạn sau <strong>1 giờ</strong>.
                </p>
                <p style='margin: 20px 0; font-size: 16px; line-height: 1.5; color: #555555;'>
                    Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.
                </p>
            </td>
        </tr>
        <tr>
            <td bgcolor='#f4f4f4' style='padding: 20px 30px; text-align: center;'>
                <p style='margin: 0; font-size: 14px; color: #888888;'>
                    Bạn nhận được email này vì đã đăng ký tài khoản tại Blood Donation Support System.
                </p>
                <p style='margin: 10px 0 0; font-size: 14px; color: #888888;'>
                    &copy; {DateTime.Now.Year} Blood Donation Support System. All rights reserved.
                </p>
            </td>
        </tr>
    </table>
</body>
</html>";

            return emailTemplate;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            // 1. Lấy thông tin người dùng từ DB
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false; // Không tìm thấy người dùng
            }

            // 2. Mã hóa mật khẩu hiện tại mà người dùng nhập và so sánh với mật khẩu trong DB
            var hashedCurrentPassword = EncryptPassword(currentPassword);
            if (user.PasswordHash != hashedCurrentPassword)
            {
                // Ném ra lỗi để Controller có thể bắt và thông báo cụ thể cho người dùng
                throw new InvalidOperationException("Mật khẩu hiện tại không chính xác.");
            }

            // 3. Nếu mật khẩu hiện tại đúng, cập nhật mật khẩu mới (đã được mã hóa)
            user.PasswordHash = EncryptPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserBloodTypeAsync(int userId, int bloodTypeId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            if (bloodTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bloodTypeId), "Blood Type ID must be greater than zero");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.BloodTypeId = bloodTypeId;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserBloodTypeByDonorIdAsync(int donorId, int bloodTypeId)
        {
            if (donorId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(donorId), "Donor ID must be greater than zero");
            }

            if (bloodTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bloodTypeId), "Blood Type ID must be greater than zero");
            }

            var user = await _userRepository.GetByIdAsync(donorId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Donor with ID {donorId} not found");
            }

            if (user.RoleId != 3)
            {
                throw new InvalidOperationException($"User with ID {donorId} is not a donor");
            }

            user.BloodTypeId = bloodTypeId;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

      
    }
}
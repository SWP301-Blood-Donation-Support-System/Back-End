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

        public UserServices(IUserRepository userRepository, IMapper mapper, IOptionsMonitor<AppSetting> options, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appSetting = options.CurrentValue;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _emailService = emailService;
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
                existingUser = await _userRepository.GetByUsernameAsync(donor.Username);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Username already exists");
                }
                User EntityUser = _mapper.Map<User>(donor);

                // Remove image processing - set UserImage to null
                EntityUser.UserImage = null;

                EntityUser.PasswordHash = EncryptPassword(donor.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 3;
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();

                // Send welcome email after successful registration
                SendWelcomeEmail(donor.Email, donor.Username);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error adding user: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
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
                existingUser = await _userRepository.GetByUsernameAsync(staff.Username);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Username already exists");
                }
                User EntityUser = _mapper.Map<User>(staff);

                // Remove image processing - set UserImage to null
                EntityUser.UserImage = null;

                EntityUser.PasswordHash = EncryptPassword(staff.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 2; // Assuming 2 is the role ID for staff
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();

                // Send welcome email after successful registration
                SendWelcomeEmail(staff.Email, staff.Username);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error adding staff: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
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

                // Remove image processing - set UserImage to null
                EntityUser.UserImage = null;

                EntityUser.PasswordHash = EncryptPassword(admin.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 1; // Assuming 1 is the role ID for admin
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();

                // Send welcome email after successful registration
                SendWelcomeEmail(admin.Email, admin.Username);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error adding admin: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
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
                        new Claim("UserName", user.Username ?? ""),
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
                    Audience = new[] { "439095486459-gvdm000c5lstr8v0j1cl3ng9bg4gs4l2.apps.googleusercontent.com" }
                });
                string email = payload.Email;
                var user = (await _userRepository.GetAllAsync()).FirstOrDefault(p => p.Email == email);

                if (user == null)
                {
                    RegisterDTO googleDTO = new RegisterDTO
                    {
                        Email = payload.Email,
                        PasswordHash = EncryptPassword(Guid.NewGuid().ToString()),
                    };
                    await RegisterDonorAsync(googleDTO);
                    user = (await _userRepository.GetAllAsync()).FirstOrDefault(p => p.Email == email);
                    LoginDTO userLogin = _mapper.Map<LoginDTO>(user);
                    await _userRepository.SaveChangesAsync();

                    // Send welcome email for new Google user
                    SendWelcomeEmail(payload.Email, payload.Name ?? payload.Email);

                    return await GenerateToken(userLogin);
                }
                else
                {
                    LoginDTO userLogin = _mapper.Map<LoginDTO>(user);
                    userLogin.PasswordHash = DecryptPassword(userLogin.PasswordHash);
                    return await GenerateToken(userLogin);
                }
            }
            catch (InvalidJwtException)
            {
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

        public void SendWelcomeEmail(string userEmail, string userName = "")
        {
            try
            {
                var subject = "Chào mừng bạn đến với Hệ thống Hỗ trợ Hiến máu!";
                var htmlBody = GenerateWelcomeEmailTemplate(userName, userEmail);

                var message = new Message(
                    to: new string[] { userEmail },
                    subject: subject,
                    content: htmlBody);

                _emailService.SendEmail(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending welcome email to {userEmail}: {ex.Message}");
            }
        }

        public void SendDonationRegistrationThankYouEmail(string userEmail, string userName, DonationRegistrationEmailInfoDTO registrationInfo)
        {
            try
            {
                var subject = "Cảm ơn bạn đã đăng ký hiến máu tình nguyện!";
                var htmlBody = GenerateDonationRegistrationThankYouEmailTemplate(userName, userEmail, registrationInfo);

                var message = new Message(
                    to: new string[] { userEmail },
                    subject: subject,
                    content: htmlBody);

                _emailService.SendEmail(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending donation registration thank you email to {userEmail}: {ex.Message}");
            }
        }

        private string GenerateWelcomeEmailTemplate(string userName, string userEmail)
        {
            var displayName = !string.IsNullOrEmpty(userName) ? userName : userEmail;
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

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

        private string GenerateDonationRegistrationThankYouEmailTemplate(string userName, string userEmail, DonationRegistrationEmailInfoDTO registrationInfo)
        {
            var displayName = !string.IsNullOrEmpty(userName) ? userName : registrationInfo.DonorName ?? userEmail;
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

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

            // Registration confirmation info
            sb.AppendLine("            <div class='registration-info'>");
            sb.AppendLine("                <h3>📋 Thông tin đăng ký</h3>");
            sb.AppendLine("                <table>");
            sb.AppendLine($"                    <tr><td class='label'>Mã đăng ký:</td><td class='important'>{registrationInfo.RegistrationCode ?? registrationInfo.RegistrationId.ToString()}</td></tr>");
            sb.AppendLine($"                    <tr><td class='label'>Ngày đăng ký:</td><td>{registrationInfo.RegistrationDate.ToString("dd/MM/yyyy HH:mm")}</td></tr>");
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

            // Schedule information
            sb.AppendLine("            <div class='schedule-info'>");
            sb.AppendLine("                <h3>📅 Thông tin lịch hiến máu</h3>");
            sb.AppendLine("                <table>");
            sb.AppendLine($"                    <tr><td class='label'>Ngày hiến máu:</td><td class='important'>{registrationInfo.ScheduleDate.ToString("dddd, dd/MM/yyyy")}</td></tr>");

            if (!string.IsNullOrEmpty(registrationInfo.TimeSlotName))
            {
                sb.AppendLine($"                    <tr><td class='label'>Khung giờ:</td><td>{registrationInfo.TimeSlotName}</td></tr>");
            }

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

            // Important notes
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

            // Call to action
            sb.AppendLine("            <div class='highlight'>");
            sb.AppendLine("                <h3>📞 Liên hệ hỗ trợ</h3>");
            sb.AppendLine("                <p>Nếu bạn có bất kỳ thắc mắc nào hoặc cần thay đổi lịch hẹn, vui lòng liên hệ:</p>");
            sb.AppendLine("                <ul>");
            sb.AppendLine("                    <li>Email: support@blooddonation.vn</li>");
            sb.AppendLine("                    <li>Hotline: 1900-XXX-XXX</li>");
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
                return true;

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var resetLink = $"http://localhost:3000/reset-password?token={token}";

            var subject = "Yêu cầu đặt lại mật khẩu";
            var body = $"<p>Xin chào {user.FullName ?? user.Username},</p>" +
               "<p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>" +
               $"<p>Vui lòng nhấp vào đường link sau để đặt lại mật khẩu. Đường link này sẽ hết hạn sau 1 giờ:</p>" +
               $"<p><a href='{resetLink}'>Đặt lại mật khẩu</a></p>" +
               "<p>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>" +
               "<p>Trân trọng,<br/>Đội ngũ Blood Donation Support System</p>";

            SendMail(subject, body, user.Email);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userRepository.GetByPasswordResetToken(token);

            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return false; // Invalid token or token expired
            }

            user.PasswordHash = EncryptPassword(newPassword);

            user.PasswordResetToken = null; // Clear the token after successful reset
            user.ResetTokenExpires = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
}
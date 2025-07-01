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
                User EntityUser = _mapper.Map<User>(donor);

                // Convert IFormFile to byte array if UserImage is provided
                if (donor.UserImage != null && donor.UserImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await donor.UserImage.CopyToAsync(memoryStream);
                        EntityUser.UserImage = memoryStream.ToArray();
                    }
                }
                else
                {
                    EntityUser.UserImage = null;
                }

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
                User EntityUser = _mapper.Map<User>(staff);
                
                // Convert IFormFile to byte array if UserImage is provided
                if (staff.UserImage != null && staff.UserImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await staff.UserImage.CopyToAsync(memoryStream);
                        EntityUser.UserImage = memoryStream.ToArray();
                    }
                }
                else
                {
                    EntityUser.UserImage = null;
                }
                
                EntityUser.PasswordHash = EncryptPassword(staff.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 2; // 2 is the role ID for staff
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
                EntityUser.PasswordHash = EncryptPassword(admin.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 1; // 1 is the role ID for admin
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding admin: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
            }
        }
        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }
            if (roleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roleId), "Role ID must be greater than zero");
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.RoleId = roleId;
            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            // Update the timestamp
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
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
                var subject = "Cảm ơn bạn đã đăng ký hiến máu tình nguyện!";
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

        private string GenerateWelcomeEmailTemplate(string userName, string userEmail)
        {
            var displayName = !string.IsNullOrEmpty(userName) ? userName : userEmail;
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            
            return "<!DOCTYPE html>" +
                   "<html lang='vi'>" +
                   "<head>" +
                   "<meta charset='UTF-8'>" +
                   "<title>Cảm ơn bạn đã đăng ký hiến máu</title>" +
                   "<style>" +
                   "body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }" +
                   ".container { max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; }" +
                   ".header { background-color: #B22222; color: white; padding: 30px 20px; text-align: center; }" +
                   ".content { padding: 30px 20px; }" +
                   ".greeting { font-size: 24px; color: #B22222; margin-bottom: 20px; font-weight: bold; }" +
                   ".highlight-box { background-color: #f8f9fa; padding: 25px; border-left: 5px solid #B22222; margin: 25px 0; }" +
                   ".footer { background-color: #333; color: white; text-align: center; padding: 20px; }" +
                   "</style>" +
                   "</head>" +
                   "<body>" +
                   "<div class='container'>" +
                   "<div class='header'>" +
                   "<h1>🩸 HIẾN MÁU CỨU NGƯỜI 🩸</h1>" +
                   "<p>Hệ thống Hỗ trợ Hiến máu Tình nguyện</p>" +
                   "</div>" +
                   "<div class='content'>" +
                   $"<div class='greeting'>Xin chào {displayName}!</div>" +
                   "<p>Trước tiên, chúng tôi xin gửi lời cảm ơn chân thành nhất đến bạn vì đã đăng ký trở thành người hiến máu tình nguyện.</p>" +
                   "<div class='highlight-box'>" +
                   "<h3>🎯 Thông tin đăng ký của bạn:</h3>" +
                   $"<p><strong>Email:</strong> {userEmail}</p>" +
                   $"<p><strong>Thời gian đăng ký:</strong> {currentDate}</p>" +
                   "<p><strong>Trạng thái:</strong> Đã đăng ký thành công</p>" +
                   "</div>" +
                   "<div class='highlight-box'>" +
                   "<h3>💝 Những lợi ích khi hiến máu:</h3>" +
                   "<ul>" +
                   "<li>Cứu sống tới 3 người bệnh chỉ với một lần hiến máu</li>" +
                   "<li>Kiểm tra sức khỏe miễn phí trước khi hiến máu</li>" +
                   "<li>Nhận giấy chứng nhận hiến máu tình nguyện</li>" +
                   "<li>Góp phần vào công tác xã hội ý nghĩa</li>" +
                   "</ul>" +
                   "</div>" +
                   "<p style='text-align: center; font-style: italic; color: #B22222;'>\"Hiến máu cứu người - Một nghĩa cử cao đẹp\"</p>" +
                   "<p>Một lần nữa, chúng tôi xin cảm ơn bạn vì tấm lòng nhân ái và sự đóng góp ý nghĩa này.</p>" +
                   "</div>" +
                   "<div class='footer'>" +
                   "<p>© 2024 Hệ thống Hỗ trợ Hiến máu Tình nguyện</p>" +
                   "<p>Email này được gửi tự động, vui lòng không phản hồi</p>" +
                   "</div>" +
                   "</div>" +
                   "</body>" +
                   "</html>";
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

            // Handle image update if provided
            if (donor.UserImage != null && donor.UserImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await donor.UserImage.CopyToAsync(memoryStream);
                    existingUser.UserImage = memoryStream.ToArray();
                }
            }
            // Note: If no image is provided, the existing image is preserved

            // Set updated timestamp
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _userRepository.UpdateAsync(existingUser);
            await _userRepository.SaveChangesAsync();

            return existingUser;
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
    }
}
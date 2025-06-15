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
                EntityUser.PasswordHash = EncryptPassword(donor.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 3;
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();
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
                EntityUser.PasswordHash = EncryptPassword(staff.PasswordHash);
                EntityUser.IsActive = true;
                EntityUser.RoleId = 2; // Assuming 2 is the role ID for staff
                await _userRepository.AddAsync(EntityUser);
                await _userRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error adding staff: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
            }
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

            bool result = await _userRepository.UpdateUserDonationAvailabilityAsync(userId,donationAvailabililtyId);
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
                        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                        new Claim("FullName", user.FullName ?? ""),
                        new Claim("DateOfBirth", user.DateOfBirth?.ToString("yyyy-MM-dd") ?? ""),
                        new Claim("RoleID", user.RoleId.ToString()),
                        new Claim("NationID", user.NationalId ?? ""),
                        new Claim("Address", user.Address ?? ""),
                        new Claim("GenderID", user.GenderId?.ToString() ?? ""),
                        new Claim("OccupationID", user.OccupationId?.ToString() ?? ""),
                        new Claim("BloodTypeID", user.BloodTypeId?.ToString() ?? ""),
                        new Claim("LastDonationDate", user.LastDonationDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                        new Claim("NextEligibleDonationDate", user.NextEligibleDonationDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                        new Claim("DonationCount", user.DonationCount?.ToString() ?? "0"),
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
                    Audience = new[] { "439095486459-gvdm000c5lstr8v0j1cl3ng9bg4gs4l2.apps.googleusercontent.com" } // Thay bằng client ID của bạn
                    
                });
                string email = payload.Email;
                var user = (await _userRepository.GetAllAsync()).FirstOrDefault(p => p.Email == email);

                if (user == null)
              
                {
                    RegisterDTO googleDTO = new RegisterDTO
                    {
                        Email = payload.Email,
                        //FullName = payload.Name,
                        PasswordHash = EncryptPassword(Guid.NewGuid().ToString()),
                        //NationalId = string.Empty,
                        //PhoneNumber = string.Empty,
                        //Username = string.Empty


                    };
                    await RegisterDonorAsync(googleDTO);
                    user = (await _userRepository.GetAllAsync()).FirstOrDefault(p => p.Email == email);
                    LoginDTO userLogin = _mapper.Map<LoginDTO>(user);
                    await _userRepository.SaveChangesAsync();
                    return await GenerateToken(userLogin);
                    

                }
                else
                {
                    LoginDTO userLogin = _mapper.Map<LoginDTO>(user);
                    userLogin.PasswordHash = DecryptPassword(userLogin.PasswordHash); // Generate a new password hash for security
                    //thank you Quang
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
                 to: new string[] {
               receiver
                 },
                 subject: mailSubject,
                 content: mailBody);

            _emailService.SendEmail(message);
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
    }

}
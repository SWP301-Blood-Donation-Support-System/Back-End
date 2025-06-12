using AutoMapper;
using BloodDonationSupportSystem.Utils;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public UserServices(IUserRepository userRepository, IMapper mapper, IOptionsMonitor<AppSetting> options, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appSetting = options.CurrentValue;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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

        public async Task AddUserAsync(UserDTO user)
        {
            try
            {
                User EntityUser = _mapper.Map<User>(user);
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

        public async Task<bool> UpdateUserStatusAsync(int userId, bool isActive)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            bool result = await _userRepository.UpdateUserStatusAsync(userId, isActive);
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
                if (user == null || user.PasswordHash != login.Password)
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
    }
}
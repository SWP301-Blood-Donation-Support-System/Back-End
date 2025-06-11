using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserServices(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;

namespace BusinessLayer.Service
{
    public class DonationRegistrationService : IDonationRegistrationServices
    {
        private readonly IDonationRegistrationRepository _donationRegistrationRepository;
        private readonly IMapper _mapper;

        public DonationRegistrationService(IDonationRegistrationRepository donationRegistrationRepository, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _donationRegistrationRepository = donationRegistrationRepository ?? throw new ArgumentNullException(nameof(donationRegistrationRepository));
        }

        public async Task<DonationRegistration> GetRegistrationByIdAsync(int registrationId)
        {
            if (registrationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(registrationId), "Registration ID must be greater than zero");
            }
            
            return await _donationRegistrationRepository.GetByIdAsync(registrationId);
        }
        
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByDonorIdAsync(int donorId)
        {
            if (donorId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(donorId), "Donor ID must be greater than zero");
            }

            return await _donationRegistrationRepository.GetRegistrationsByDonorIdAsync(donorId);
        }
        
        public async Task<IEnumerable<DonationRegistration>> GetAllRegistrationsAsync()
        {
            return await _donationRegistrationRepository.GetAllAsync();
        }
        
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleIdAsync(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scheduleId), "Schedule ID must be greater than zero");
            }
            return await _donationRegistrationRepository.GetRegistrationsByScheduleIdAsync(scheduleId);
        }
        
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByStatusIdAsync(int statusId)
        {
            return await _donationRegistrationRepository.GetRegistrationsByStatusIdAsync(statusId);
        }
        
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId)
        {
            if (timeSlotId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSlotId), "Timeslot ID must be greater than zero");
            }
            return await _donationRegistrationRepository.GetRegistrationsByTimeSlotIdAsync(timeSlotId);
        }
        
        public async Task AddRegistrationAsync(DonationRegistrationDTO registration)
        {
            try
            {
               var entity = _mapper.Map<DonationRegistration>(registration);
                entity.RegistrationStatusId = 1; // 1 is the default status for new registrations
                await _donationRegistrationRepository.AddAsync(entity);
                await _donationRegistrationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error adding registration: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller
            }
        }
       

        public async Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId)
        {
            return await _donationRegistrationRepository.UpdateRegistrationStatusAsync(registrationId, statusId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _donationRegistrationRepository.SaveChangesAsync();
        }

        public Task<bool> SoftDeleteRegistrationAsync(int registrationId)
        {
            return _donationRegistrationRepository.SoftDeleteRegistrationAsync(registrationId);
        }
    }
}

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

        public async Task<DonationRegistration> GetRegistrationsByDonorIdAsync(int donorId)
        {
            if (donorId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(donorId), "Donor ID must be greater than zero");
            }

            return await _donationRegistrationRepository.GetByIdAsync(donorId);
        }
        public async Task<IEnumerable<DonationRegistration>> GetAllRegistrationsAsync()
        {
            return await _donationRegistrationRepository.GetAllAsync();
        }
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationByScheduleIdAsync(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scheduleId), "Schedule ID must be greater than zero");
            }
            return await _donationRegistrationRepository.GetRegistrationsByScheduleIdAsync(scheduleId);
        }
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByStatusIdAsync(int statusId)
        {
            if (statusId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(statusId), "Status ID must be greater than zero");
            }
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
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByQrCodeAsync(string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                throw new ArgumentNullException(nameof(qrCode), "QR Code cannot be null or empty");
            }
            return await _donationRegistrationRepository.GetRegistrationsByQrCodeAsync(qrCode);
        }
        public async Task AddRegistrationAsync(DonationRegistrationDTO registration)
        {
            try
            {
               var entity = _mapper.Map<DonationRegistration>(registration);
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

        public Task<bool> DeleteRegistrationAsync(int registrationId)
        {
            throw new NotImplementedException();
        }

        public Task<DonationRegistration> GetRegistrationByIdAsync(int registrationId)
        {
            throw new NotImplementedException();
        }

        public Task<DonationRegistration> GetRegistrationByTimeSlotIdAsync(int timeSlotId)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleIdAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> SaveChangesAsync()
        {
            
            return await _donationRegistrationRepository.SaveChangesAsync();
        }

        public Task<DonationRegistration> UpdateRegistrationAsync(DonationRegistration registration)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<DonationRegistration>> IDonationRegistrationServices.GetRegistrationsByDonorIdAsync(int donorId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<DonationRegistration>> IDonationRegistrationServices.GetRegistrationByTimeSlotIdAsync(int timeSlotId)
        {
            throw new NotImplementedException();
        }
    }
}

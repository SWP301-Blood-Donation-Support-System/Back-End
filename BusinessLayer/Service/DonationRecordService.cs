using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;

namespace BusinessLayer.Service
{
    public class DonationRedordService : IDonationRedordService
    {
        private readonly IDonationRecordRepository _donationRecordRepository;
        private readonly IMapper _mapper;

        public DonationRecordService(IDonationRecordRepository donationRegistrationRepository, IMapper mapper)
        {
            _donationRecordRepository = donationRegistrationRepository ?? throw new ArgumentNullException(nameof(donationRegistrationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DonationRegistration> GetRegistrationByIdAsync(int registrationId)
        {
            if (registrationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(registrationId), "Registration ID must be greater than zero");
            }

            return await _donationRegistrationRepository.GetByIdAsync(registrationId);
        }

        public async Task<IEnumerable<DonationRegistration>> GetAllRegistrationsAsync()
        {
            return await _donationRegistrationRepository.GetAllAsync();
        }

        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByDonorIdAsync(int donorId)
        {
            if (donorId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(donorId), "Donor ID must be greater than zero");
            }

            return await _donationRegistrationRepository.GetRegistrationsByDonorIdAsync(donorId);
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
            if (statusId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(statusId), "Status ID must be greater than zero");
            }
            return await _donationRegistrationRepository.GetRegistrationsByStatusIdAsync(statusId);
        }

        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByQrCodeAsync(string qrCode)
        {
            if (string.IsNullOrWhiteSpace(qrCode))
            {
                throw new ArgumentException("QR Code cannot be null or empty", nameof(qrCode));
            }
            return await _donationRegistrationRepository.GetRegistrationsByQrCodeAsync(qrCode);
        }

        public async Task<IEnumerable<DonationRegistration>> GetRegistrationByTimeSlotIdAsync(int timeSlotId)
        {
            if (timeSlotId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSlotId), "Time slot ID must be greater than zero");
            }
            return await _donationRegistrationRepository.GetRegistrationsByTimeSlotIdAsync(timeSlotId);
        }

        public async Task<DonationRegistration> AddRegistrationAsync(DonationRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration), "Registration cannot be null");
            }

            // Set creation timestamp
            registration.CreatedAt = DateTime.UtcNow;
            registration.UpdatedAt = DateTime.UtcNow;

            var addedRecord = await _donationRecordRepository.AddAsync(registration);
            await _donationRecordRepository.SaveChangesAsync();
            return addedRecord;
        }

        public async Task<DonationRegistration> UpdateRegistrationAsync(DonationRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration), "Registration cannot be null");
            }

            // Update timestamp
            registration.UpdatedAt = DateTime.UtcNow;

            await _donationRegistrationRepository.UpdateAsync(registration);
            await _donationRegistrationRepository.SaveChangesAsync();
            return registration;
        }

        public async Task<bool> DeleteRegistrationAsync(int registrationId)
        {
            if (registrationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(registrationId), "Registration ID must be greater than zero");
            }

            var result = await _donationRecordRepository.DeleteAsync(registrationId);
            if (result)
            {
                await _donationRecordRepository.SaveChangesAsync();
            }
            return result;
        }

        public async Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId)
        {
            if (registrationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(registrationId), "Registration ID must be greater than zero");
            }

            if (statusId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(statusId), "Status ID must be greater than zero");
            }

            // Get the existing registration
            var registration = await _donationRegistrationRepository.GetByIdAsync(registrationId);
            if (registration == null)
            {
                return false;
            }

            // Update the status and timestamp
            registration.StatusId = statusId;
            registration.UpdatedAt = DateTime.UtcNow;

            await _donationRegistrationRepository.UpdateAsync(registration);
            await _donationRegistrationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _donationRegistrationRepository.SaveChangesAsync();
        }
    }
}
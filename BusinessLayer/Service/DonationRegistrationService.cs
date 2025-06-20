using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;

namespace BusinessLayer.Service
{
    public class DonationRegistrationService : IDonationRegistrationServices
    {
        private readonly IDonationRegistrationRepository _donationRegistrationRepository;
        private readonly IDonationScheduleRepository _donationScheduleRepository;
        private readonly IMapper _mapper;

        public DonationRegistrationService(
            IDonationRegistrationRepository donationRegistrationRepository,
            IDonationScheduleRepository donationScheduleRepository,
            IMapper mapper)
        {
            _donationRegistrationRepository = donationRegistrationRepository ?? throw new ArgumentNullException(nameof(donationRegistrationRepository));
            _donationScheduleRepository = donationScheduleRepository ?? throw new ArgumentNullException(nameof(donationScheduleRepository));
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
                var existingRegistrations = await _donationRegistrationRepository.GetRegistrationsByDonorIdAsync(registration.DonorId);
                if (existingRegistrations.Any(r => r.RegistrationStatusId == 1))
                {
                    throw new InvalidOperationException("You already have an active donation registration. Please complete or cancel it before registering again.");
                }

                var schedule = await _donationScheduleRepository.GetByIdAsync(registration.ScheduleId);
                if (schedule == null)
                {
                    throw new InvalidOperationException($"Donation schedule with ID {registration.ScheduleId} does not exist.");
                }

                if (schedule.IsDeleted)
                {
                    throw new InvalidOperationException($"Donation schedule with ID {registration.ScheduleId} is no longer available.");
                }

                var entity = _mapper.Map<DonationRegistration>(registration);
                entity.RegistrationStatusId = 1;
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.CreatedBy = "System";

                schedule.RegisteredSlots += 1;
                schedule.UpdatedAt = DateTime.UtcNow;
                schedule.UpdatedBy = "System";

                await _donationRegistrationRepository.AddAsync(entity);
                await _donationScheduleRepository.UpdateAsync(schedule);

                var saveResult = await _donationRegistrationRepository.SaveChangesAsync();

                if (!saveResult)
                {
                    throw new Exception("Failed to save changes to the database");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding registration: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId)
        {
            var registration = await _donationRegistrationRepository.GetRegistrationWithDonorAndRecordAsync(registrationId);
            
            if (registration == null || registration.IsDeleted)
            {
                return false;
            }
            
            registration.RegistrationStatusId = statusId;
            registration.UpdatedAt = DateTime.UtcNow;
            
            if (statusId == 3 && registration.Donor != null && registration.DonationRecord != null)
            {
                var donor = registration.Donor;
                var donationRecord = registration.DonationRecord;

                if (string.IsNullOrEmpty(donationRecord.CertificateId))
                {
                    string year = donationRecord.DonationDateTime.Year.ToString();
                    string month = donationRecord.DonationDateTime.Month.ToString().PadLeft(2, '0');
                    string donorIdPart = donor.UserId.ToString().PadLeft(5, '0');

                    int donationCount = donor.DonationCount ?? 0;
                    string counter = (donationCount + 1).ToString().PadLeft(3, '0');

                    donationRecord.CertificateId = $"BDC-{donorIdPart}-{year}{month}-{counter}";
                    donationRecord.UpdatedAt = DateTime.UtcNow;
                }

                donor.LastDonationDate = donationRecord.DonationDateTime;
                
                int donationTypeId = donationRecord.DonationTypeId ?? 1;
                
                if (donationTypeId == 1 || donationTypeId == 4)
                {
                    donor.NextEligibleDonationDate = donor.LastDonationDate.Value.AddDays(84); 
                }
                else if (donationTypeId == 2 || donationTypeId == 3)
                {
                    donor.NextEligibleDonationDate = donor.LastDonationDate.Value.AddDays(14);
                }
                
                donor.DonationCount += 1;
                donor.DonationAvailabilityId = 2;
            }
            
            await _donationRegistrationRepository.UpdateAsync(registration);
            return await _donationRegistrationRepository.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _donationRegistrationRepository.SaveChangesAsync();
        }

        public Task<bool> SoftDeleteRegistrationAsync(int registrationId)
        {
            return _donationRegistrationRepository.SoftDeleteRegistrationAsync(registrationId);
        }

        public async Task<DonationRegistration?> CheckInByNationalIdAsync(string nationalId, int approvedStatusId, int checkedInStatusId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
            {
                throw new ArgumentException("National ID cannot be null or empty", nameof(nationalId));
            }

            // First check if there's already a checked-in registration for today for this national ID
            var existingCheckedIn = await _donationRegistrationRepository
                .GetTodayRegistrationByNationalIdAsync(nationalId, checkedInStatusId);
            
            if (existingCheckedIn != null)
            {
                // User is already checked in today
                return existingCheckedIn;
            }

            // Try to find an approved registration for today
            var registration = await _donationRegistrationRepository
                .GetTodayRegistrationByNationalIdAsync(nationalId, approvedStatusId);

            if (registration == null)
            {
                // No approved registration found for today
                return null;
            }

            // Update the status to checked-in
            registration.RegistrationStatusId = checkedInStatusId;
            registration.UpdatedAt = DateTime.UtcNow;
            
            await _donationRegistrationRepository.UpdateAsync(registration);
            await _donationRegistrationRepository.SaveChangesAsync();
            
            return registration;
        }

        public async Task<IEnumerable<DonationRegistration>> GetByScheduleAndTimeSlotAsync(int scheduleId, int timeSlotId)
        {
            return await _donationRegistrationRepository.GetByScheduleAndTimeSlotAsync(scheduleId, timeSlotId);
        }

        public async Task<DonationRegistration?> GetTodayRegistrationByNationalIdAsync(string nationalId, int approvedStatusId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
            {
                throw new ArgumentException("National ID cannot be null or empty", nameof(nationalId));
            }

            return await _donationRegistrationRepository.GetTodayRegistrationByNationalIdAsync(nationalId, approvedStatusId);
        }

        // DTO-based methods to prevent circular references
        public async Task<IEnumerable<DonationRegistrationResponseDTO>> GetAllRegistrationsResponseAsync()
        {
            var registrations = await _donationRegistrationRepository.GetAllAsync();
            var registrationDTOs = _mapper.Map<IEnumerable<DonationRegistrationResponseDTO>>(registrations);
            return registrationDTOs;
        }

        public async Task<DonationRegistrationResponseDTO> GetRegistrationByIdResponseAsync(int registrationId)
        {
            var registration = await _donationRegistrationRepository.GetByIdAsync(registrationId);
            return _mapper.Map<DonationRegistrationResponseDTO>(registration);
        }

        public async Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByDonorIdResponseAsync(int donorId)
        {
            var registrations = await _donationRegistrationRepository.GetRegistrationsByDonorIdAsync(donorId);
            return _mapper.Map<IEnumerable<DonationRegistrationResponseDTO>>(registrations);
        }

        public async Task<DonationRegistrationResponseDTO> CheckInByNationalIdResponseAsync(
            string nationalId, 
            int approvedStatusId, 
            int checkedInStatusId)
        {
            var registration = await CheckInByNationalIdAsync(nationalId, approvedStatusId, checkedInStatusId);
            
            if (registration == null)
            {
                return null;
            }
            
            return _mapper.Map<DonationRegistrationResponseDTO>(registration);
        }

        // Implement new DTO-based methods
        public async Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByScheduleIdResponseAsync(int scheduleId)
        {
            var registrations = await _donationRegistrationRepository.GetRegistrationsByScheduleIdAsync(scheduleId);
            return _mapper.Map<IEnumerable<DonationRegistrationResponseDTO>>(registrations);
        }

        public async Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByStatusIdResponseAsync(int statusId)
        {
            var registrations = await _donationRegistrationRepository.GetRegistrationsByStatusIdAsync(statusId);
            return _mapper.Map<IEnumerable<DonationRegistrationResponseDTO>>(registrations);
        }

        public async Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByTimeSlotIdResponseAsync(int timeSlotId)
        {
            var registrations = await _donationRegistrationRepository.GetRegistrationsByTimeSlotIdAsync(timeSlotId);
            return _mapper.Map<IEnumerable<DonationRegistrationResponseDTO>>(registrations);
        }

        public async Task<IEnumerable<DonationRegistrationResponseDTO>> GetByScheduleAndTimeSlotResponseAsync(int scheduleId, int timeSlotId)
        {
            var registrations = await _donationRegistrationRepository.GetByScheduleAndTimeSlotAsync(scheduleId, timeSlotId);
            return _mapper.Map<IEnumerable<DonationRegistrationResponseDTO>>(registrations);
        }
    }
}

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

        // Update the AddRegistrationAsync method in DonationRegistrationService.cs

        public async Task AddRegistrationAsync(DonationRegistrationDTO registration)
        {
            try
            {
                // Check if donor already has active registration
                var existingRegistrations = await _donationRegistrationRepository.GetRegistrationsByDonorIdAsync(registration.DonorId);
                if (existingRegistrations.Any(r => r.RegistrationStatusId == 1))
                {
                    throw new InvalidOperationException("You already have an active donation registration. Please complete or cancel it before registering again.");
                }

                // Check if the schedule exists
                var schedule = await _donationScheduleRepository.GetByIdAsync(registration.ScheduleId);
                if (schedule == null)
                {
                    throw new InvalidOperationException($"Donation schedule with ID {registration.ScheduleId} does not exist.");
                }

                // Check if the schedule is deleted
                if (schedule.IsDeleted)
                {
                    throw new InvalidOperationException($"Donation schedule with ID {registration.ScheduleId} is no longer available.");
                }

                var entity = _mapper.Map<DonationRegistration>(registration);
                entity.RegistrationStatusId = 1; // Set default status to 1 (active/pending)
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.CreatedBy = "System"; // Add a default value or from the DTO if available

                // Update the schedule directly
                schedule.RegisteredSlots += 1;
                schedule.UpdatedAt = DateTime.UtcNow;
                schedule.UpdatedBy = "System"; // Add a default value or from the DTO if available

                // Add the registration
                await _donationRegistrationRepository.AddAsync(entity);

                // Update the schedule
                await _donationScheduleRepository.UpdateAsync(schedule);

                // Save all changes
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
            // Get the registration with related donor and donation record
            var registration = await _donationRegistrationRepository.GetRegistrationWithDonorAndRecordAsync(registrationId);
            
            if (registration == null || registration.IsDeleted)
            {
                return false;
            }
            
            registration.RegistrationStatusId = statusId;
            registration.UpdatedAt = DateTime.UtcNow;
            
            // If status is "Completed" and there's a donor linked
            if (statusId == 3 && registration.Donor != null && registration.DonationRecord != null)
            {
                var donor = registration.Donor;
                var donationRecord = registration.DonationRecord;

                if (string.IsNullOrEmpty(donationRecord.CertificateId))
                {
                    // Format: BDC-{DonorID}-{Year}-{Month}-{Counter}
                    string year = donationRecord.DonationDateTime.Year.ToString();
                    string month = donationRecord.DonationDateTime.Month.ToString().PadLeft(2, '0');
                    string donorIdPart = donor.UserId.ToString().PadLeft(5, '0');

                    // Get donation count for this user as the counter
                    int donationCount = donor.DonationCount ?? 0;
                    string counter = (donationCount + 1).ToString().PadLeft(3, '0');

                    donationRecord.CertificateId = $"BDC-{donorIdPart}-{year}{month}-{counter}";
                    donationRecord.UpdatedAt = DateTime.UtcNow;
                }

                // Update last donation date to record date
                donor.LastDonationDate = donationRecord.DonationDateTime;
                
                // Calculate next eligible donation date based on donation type
                int donationTypeId = donationRecord.DonationTypeId??1;
                
                if (donationTypeId == 1 || donationTypeId == 4)
                {
                    // Types 1 and 4: Add 12 weeks (84 days)
                    donor.NextEligibleDonationDate = donor.LastDonationDate.Value.AddDays(84); 
                }
                else if (donationTypeId == 2 || donationTypeId == 3)
                {
                    // Types 2 and 3: Add 2 weeks (14 days)
                    donor.NextEligibleDonationDate = donor.LastDonationDate.Value.AddDays(14);
                }
                
                // Increment donation count
                donor.DonationCount += 1;
                donor.DonationAvailabilityId = 2;
            }
            
            // Update the registration
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
    }
}

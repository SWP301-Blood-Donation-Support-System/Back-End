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

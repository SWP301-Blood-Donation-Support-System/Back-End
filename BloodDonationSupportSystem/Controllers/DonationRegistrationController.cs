using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationRegistrationController : ControllerBase
    {
        private readonly IDonationRegistrationServices _donationRegistrationService;
        public DonationRegistrationController(IDonationRegistrationServices donationRegistrationService)
        {
            _donationRegistrationService = donationRegistrationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRegistrations()
        {
            var registrations = await _donationRegistrationService.GetAllRegistrationsAsync();
            return Ok(registrations);
        }
        [HttpPost("registerDonation")]
        public async Task<IActionResult> RegisterDonation([FromBody] DonationRegistrationDTO registrationDTO)
        {

            try
            {
                await _donationRegistrationService.AddRegistrationAsync(registrationDTO);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });

            }
            return Ok("Donation registered successfully.");

        }
        [HttpGet("getRegistrationById/{registrationId}")]
        public async Task<IActionResult> GetRegistrationById(int registrationId)
        {
            if (registrationId <= 0)
            {
                return BadRequest("Invalid registration ID.");
            }
            var registration = await _donationRegistrationService.GetRegistrationByIdAsync(registrationId);
            if (registration == null)
            {
                return NotFound($"No registration found with ID {registrationId}.");
            }
            return Ok(registration);
        }
        [HttpGet("getRegistrationsByDonorId/{donorId}")]
        public async Task<IActionResult> GetRegistrationsByDonorId(int donorId)
        {
            if (donorId <= 0)
            {
                return BadRequest("Invalid donor ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByDonorIdAsync(donorId);
            if (registrations == null)
            {
                return NotFound($"No registrations found for donor ID {donorId}.");
            }
            return Ok(registrations);
        }
        [HttpGet("getRegistrationsByScheduleId/{scheduleId}")]
        public async Task<IActionResult> GetRegistrationsByScheduleId(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                return BadRequest("Invalid schedule ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByScheduleIdAsync(scheduleId);
            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for schedule ID {scheduleId}.");
            }
            return Ok(registrations);
        }
        [HttpGet("getRegistrationsByStatusId/{statusId}")]
        public async Task<IActionResult> GetRegistrationsByStatusId(int statusId)
        {
            if (statusId <= 0)
            {
                return BadRequest("Invalid status ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByStatusIdAsync(statusId);
            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for status ID {statusId}.");
            }
            return Ok(registrations);
        }
        [HttpGet("getRegistrationsByTimeSlotId/{timeSlotId}")]
        public async Task<IActionResult> GetRegistrationsByTimeSlotId(int timeSlotId)
        {
            if (timeSlotId <= 0)
            {
                return BadRequest("Invalid time slot ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByTimeSlotIdAsync(timeSlotId);
            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for time slot ID {timeSlotId}.");
            }
            return Ok(registrations);
        }
      
        [HttpPut("updateRegistrationStatus/{registrationId}/{statusId}")]
        public async Task<IActionResult> UpdateRegistrationStatus(int registrationId, int statusId)
        {
            if (registrationId <= 0 || statusId <= 0)
            {
                return BadRequest("Invalid registration ID or status ID.");
            }
            var result = await _donationRegistrationService.UpdateRegistrationStatusAsync(registrationId, statusId);
            if (!result)
            {
                return NotFound($"No registration found with ID {registrationId} or failed to update status.");
            }
            return Ok("Registration status updated successfully.");
        }
        [HttpPatch("softDeleteRegistration/{registrationId}")]
        public async Task<IActionResult> SoftDeleteRegistration(int registrationId)
        {
            if (registrationId <= 0)
            {
                return BadRequest("Invalid registration ID.");
            }
            var result = await _donationRegistrationService.SoftDeleteRegistrationAsync(registrationId);
            if (!result)
            {
                return NotFound($"No registration found with ID {registrationId} or failed to delete.");
            }
            return Ok("Registration deleted successfully.");
        }
        [HttpPut("cancelRegistration/{registrationId}")]
        public async Task<IActionResult> CancelRegistration(int registrationId)
        {
            if (registrationId <= 0)
            {
                return BadRequest("Invalid registration ID.");
            }

            // Status ID 4 represents cancelled status
            var result = await _donationRegistrationService.UpdateRegistrationStatusAsync(registrationId, 4);
            if (!result)
            {
                return NotFound($"No registration found with ID {registrationId} or failed to cancel registration.");
            }
            return Ok("Registration cancelled successfully.");
        }
    }
}

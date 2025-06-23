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
            var registrations = await _donationRegistrationService.GetAllRegistrationsResponseAsync();
            return Ok(registrations);
        }
        [HttpGet("registration/{registrationId}")]
        public async Task<IActionResult> GetRegistrationById(int registrationId)
        {
            if (registrationId <= 0)
            {
                return BadRequest("Invalid registration ID.");
            }
            var registration = await _donationRegistrationService.GetRegistrationByIdResponseAsync(registrationId);
            if (registration == null)
            {
                return NotFound($"No registration found with ID {registrationId}.");
            }
            return Ok(registration);
        }
        [HttpGet("by-donor/{donorId}")]
        public async Task<IActionResult> GetRegistrationsByDonorId(int donorId)
        {
            if (donorId <= 0)
            {
                return BadRequest("Invalid donor ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByDonorIdResponseAsync(donorId);
            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for donor ID {donorId}.");
            }
            return Ok(registrations);
        }

        [HttpGet("by-schedule/{scheduleId}")]
        public async Task<IActionResult> GetRegistrationsByScheduleId(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                return BadRequest("Invalid schedule ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByScheduleIdResponseAsync(scheduleId);
            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for schedule ID {scheduleId}.");
            }
            return Ok(registrations);
        }

        [HttpGet("by-status/{statusId}")]
        public async Task<IActionResult> GetRegistrationsByStatusId(int statusId)
        {
            if (statusId <= 0)
            {
                return BadRequest("Invalid status ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByStatusIdResponseAsync(statusId);
            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for status ID {statusId}.");
            }
            return Ok(registrations);
        }

        [HttpGet("by-time-slot{timeSlotId}")]
        public async Task<IActionResult> GetRegistrationsByTimeSlotId(int timeSlotId)
        {
            if (timeSlotId <= 0)
            {
                return BadRequest("Invalid time slot ID.");
            }
            var registrations = await _donationRegistrationService.GetRegistrationsByTimeSlotIdResponseAsync(timeSlotId);
            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for time slot ID {timeSlotId}.");
            }
            return Ok(registrations);
        }
        [HttpGet("{timeSlotId}/{scheduleId}")]
        public async Task<IActionResult> GetRegistrationsByTimeSlotIdAndScheduleId(int timeSlotId, int scheduleId)
        {
            if (timeSlotId <= 0 || scheduleId <= 0)
            {
                return BadRequest("Invalid time slot ID or schedule ID.");
            }

            var registrations = await _donationRegistrationService.GetByScheduleAndTimeSlotResponseAsync(scheduleId, timeSlotId);

            if (registrations == null || !registrations.Any())
            {
                return NotFound($"No registrations found for time slot ID {timeSlotId} and schedule ID {scheduleId}.");
            }

            return Ok(registrations);
        }

        [HttpPost]
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
        
        
        
       
        [HttpPut("registration-status")]
        public async Task<IActionResult> UpdateRegistrationStatus([FromBody] UpdateRegistrationStatusDTO request)
        {
            if (request == null)
            {
                return BadRequest("Request data is required.");
            }

            if (request.RegistrationId <= 0 || request.StatusId <= 0)
            {
                return BadRequest("Invalid registration ID or status ID.");
            }

            var result = await _donationRegistrationService.UpdateRegistrationStatusAsync(request.RegistrationId, request.StatusId);
            if (!result)
            {
                return NotFound($"No registration found with ID {request.RegistrationId} or failed to update status.");
            }
            return Ok("Registration status updated successfully.");
        }

        
        
        [HttpPut("cancel-registration")]
        public async Task<IActionResult> CancelRegistration([FromBody] UpdateRegistrationStatusDTO request)
        {
            if (request == null)
            {
                return BadRequest("Request data is required.");
            }

            if (request.RegistrationId <= 0)
            {
                return BadRequest("Invalid registration ID.");
            }

            // Status ID 4 represents cancelled status
            var result = await _donationRegistrationService.UpdateRegistrationStatusAsync(request.RegistrationId, 4);
            if (!result)
            {
                return NotFound($"No registration found with ID {request.RegistrationId} or failed to cancel registration.");
            }
            return Ok("Registration cancelled successfully.");
        }
        
        

        [HttpPut("check-in/{nationalId}")]
        public async Task<IActionResult> CheckIn(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
            {
                return BadRequest(new { 
                    status = "failed", 
                    message = "National ID is required and cannot be empty." 
                });
            }

            try
            {
                const int approvedStatusId = 1; // Status "Approved"
                const int checkedInStatusId = 2; // Status "Checked-in"

                var checkedInRegistration = await _donationRegistrationService.CheckInByNationalIdResponseAsync(
                    nationalId,
                    approvedStatusId,
                    checkedInStatusId
                );

                if (checkedInRegistration == null)
                {
                    return NotFound(new { 
                        status = "failed", 
                        message = $"No approved registration found for today with National ID: {nationalId}." 
                    });
                }

                // Check if the status is already "checked-in"
                if (checkedInRegistration.RegistrationStatusId == checkedInStatusId)
                {
                    return Ok(new { 
                        status = "warning",
                        message = "User already checked in today.",
                        registration = checkedInRegistration 
                    });
                }

                return Ok(new { 
                    status = "success",
                    message = "Check-in successful.",
                    registration = checkedInRegistration 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    status = "failed", 
                    message = ex.Message 
                });
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error during check-in: {ex.Message}");
                return StatusCode(500, new { 
                    status = "error", 
                    message = "An internal error occurred while trying to check-in." 
                });
            }
        }
        [HttpPatch("soft-delete{registrationId}")]
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
    }
}

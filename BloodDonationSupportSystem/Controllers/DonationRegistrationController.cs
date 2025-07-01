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



        [HttpPut("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDTO request)
        {
            // 2. Không cần check null/empty thủ công cho NationalId nữa
            // vì [ApiController] và [Required] trong model đã tự động xử lý.
            // Nếu request thiếu nationalId, client sẽ tự nhận lỗi 400 Bad Request.

            try
            {
                // Sử dụng Enum để code dễ đọc hơn
                const int approvedStatusId = 1; // Hoặc (int)RegistrationStatus.Approved
                const int checkedInStatusId = 2; // Hoặc (int)RegistrationStatus.CheckedIn

                // 3. Gọi phương thức service mới, có thể truyền vào cả ScheduleId để xử lý chính xác hơn
                // Service sẽ tìm đăng ký có NationalId và ScheduleId tương ứng, đang ở trạng thái Approved, diễn ra trong ngày hôm nay.
                var checkedInRegistration = await _donationRegistrationService.CheckInByNationalIdResponseAsync(
                    request.NationalId,
                    approvedStatusId,
                    checkedInStatusId
                );

                // 4. Toàn bộ cấu trúc response (success, warning, failed) được giữ nguyên
                if (checkedInRegistration == null)
                {
                    return NotFound(new
                    {
                        status = "failed",
                        message = $"Không tìm thấy đăng ký hợp lệ (trạng thái 'Approved') cho ngày hôm nay với CCCD/CMND: {request.NationalId} tại lịch hiến máu này."
                    });
                }

                // Đoạn logic này vẫn có thể hữu ích nếu service của bạn trả về thông tin
                // ngay cả khi người dùng đã check-in rồi, để controller quyết định thông báo.
                if (checkedInRegistration.RegistrationStatusId == checkedInStatusId)
                {
                    return Ok(new
                    {
                        status = "warning",
                        message = "Người dùng đã điểm danh (check-in) trong hôm nay rồi.",
                        registration = checkedInRegistration
                    });
                }

                return Ok(new
                {
                    status = "success",
                    message = "Check-in thành công.",
                    registration = checkedInRegistration
                });
            }
            catch (ArgumentException ex) // Lỗi do đầu vào không hợp lệ từ service
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Log the exception details (quan trọng cho việc debug)
                Console.WriteLine($"Error during check-in: {ex.ToString()}");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Đã có lỗi hệ thống xảy ra trong quá trình check-in."
                });
            }
        }
        [HttpDelete]
        //[ProducesResponseType(204)] // No Content
        public async Task<IActionResult> SoftDeleteRegistration([FromBody] SoftDeleteRegistrationDTO request)
        {
            var success = await _donationRegistrationService.SoftDeleteRegistrationAsync(request.RegistrationId);

            if (!success)
            {
                return NotFound($"Không tìm thấy đăng ký với ID {request.RegistrationId} để xóa.");
            }

            return NoContent(); // Xóa thành công, không cần trả về nội dung
        }
    }
}

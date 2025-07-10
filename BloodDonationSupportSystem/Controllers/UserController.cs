using BusinessLayer.IService;
using BusinessLayer.Utils;
using DataAccessLayer.DTO;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userServices.GetAllUsersAsync();
            return Ok(users);
        }

        // DELETE: api/user/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { status = "failed", message = "Invalid user ID" });
            }

            try
            {
                var user = await _userServices.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new { status = "failed", message = $"User with ID {id} not found" });
                }

                return Ok(user);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "failed", message = "An error occurred while retrieving the user" });
            }
        }
        [HttpGet("by-role/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            if (roleId <= 0)
            {
                return BadRequest("Invalid role ID.");
            }

            var users = await _userServices.GetUsersByRoleAsync(roleId);
            return Ok(users);
        }
        [HttpGet("by-blood-type/{bloodTypeId}")]
        public async Task<IActionResult> GetUsersByBloodType(int bloodTypeId)
        {
            if (bloodTypeId <= 0)
            {
                return BadRequest("Invalid blood type ID.");
            }
            var users = await _userServices.GetUsersByBloodTypeAsync(bloodTypeId);
            return Ok(users);
        }
        [HttpGet("eligible-donor")]
        public async Task<IActionResult> GetEligibleDonors()
        {
            var eligibleDonors = await _userServices.GetEligibleDonorsAsync();
            return Ok(eligibleDonors);
        }
        /// <summary>
        /// Login user and generate JWT token
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.PasswordHash))
                {
                    return BadRequest(new { status = "failed", message = "Email and password are required" });
                }

                var token = await _userServices.GenerateToken(login);
                if (!string.IsNullOrEmpty(token))
                {
                    return new JsonResult(new
                    {
                        result = token
                    });
                }
                return NotFound(new { status = "failed", message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return new JsonResult(new
                {
                    status = "failed",
                    message = "An error occurred during login"
                });
            }
        }
        /// <summary>
        /// Register account for donor
        /// </summary>
        /// <param name="registerDTO"></param>
        /// <returns></returns>
        [HttpPost("register-donor")]
        public async Task<IActionResult> RegisterDonor([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                await _userServices.RegisterDonorAsync(registerDTO);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });

            }
            return Ok("Donor registered successfully.");

        }
        /// <summary>
        /// Register account for staff
        /// </summary>
        /// <param name="staffRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost("register-staff")]
        public async Task<IActionResult> RegisterStaff([FromBody] StaffRegisterDTO staffRegisterDTO)
        {
            try
            {
                await _userServices.RegisterStaffAsync(staffRegisterDTO);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });

            }
            return Ok("Staff registered successfully.");
        }
        /// <summary>
        /// Register account for admin
        /// </summary>
        /// <param name="adminRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] StaffRegisterDTO adminRegisterDTO)
        {
            try
            {
                await _userServices.RegisterAdminAsync(adminRegisterDTO);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
            return Ok("Admin registered successfully.");
        }
        /// <summary>
        /// Register account for hospital
        /// </summary>
        /// <param name="hospitalRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost("register-hospital")]
        public async Task<IActionResult> RegisterHospital([FromBody] HospitalRegisterDTO hospitalRegisterDTO)
        {
            try
            {
                await _userServices.RegisterHospitalAsync(hospitalRegisterDTO);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
            return Ok("Hospital registered successfully.");
        }
        /// <summary>
        /// Update user role by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPatch("{userId}/role")]
        public async Task<IActionResult> SwitchRole(int userId, [FromBody] int roleId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user data.");
            }
            try
            {
                var updatedUser = await _userServices.UpdateUserRoleAsync(userId, roleId);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
        }
        /// <summary>
        /// Login or register using Google
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("google")]
        public async Task<IActionResult> VerifyGoogleToken([FromBody] TokenRequest request)
        {

            var token = await _userServices.ValidateGoogleToken(request);
            if (!string.IsNullOrEmpty(token))
            {
                return new JsonResult(new
                {
                    result = token
                });
            }
            return NotFound(new { status = "failed", message = "Invalid" });
        }
        /// <summary>
        /// Update donor information 
        /// </summary>
        /// <param name="donorId"></param>
        /// <param name="donorDTO"></param>
        /// <returns></returns>
        [HttpPut("donor/{donorId}")]
        public async Task<IActionResult> UpdateDonor(int donorId, [FromBody] DonorDTO donorDTO)
        {
            if (donorDTO == null || donorId <= 0)
            {
                return BadRequest("Invalid donor data.");
            }
            try
            {
                var updatedDonor = await _userServices.UpdateDonorAsync(donorId, donorDTO);
                return Ok(updatedDonor);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
        }

        //[HttpPut("donation-date/{userId}")]
        //public async Task<IActionResult> UpdateDonationInfo(int userId, [FromBody] DateTime donationDate)
        //{
        //    if (userId <= 0 || donationDate == default)
        //    {
        //        return BadRequest("Invalid user ID or donation date.");
        //    }
        //    try
        //    {
        //        var result = await _userServices.UpdateDonationInfoAsync(userId, donationDate);
        //        return Ok(new { success = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = "failed", message = ex.Message });
        //    }
        //}
        //[HttpPut("donation-availability/{userId}")]
        //public async Task<IActionResult> UpdateUserDonationAvailability(int userId, [FromBody] int donationAvailabilityId)
        //{
        //    if (userId <= 0 || donationAvailabilityId <= 0)
        //    {
        //        return BadRequest("Invalid user ID or donation availability ID.");
        //    }
        //    try
        //    {
        //        var result = await _userServices.UpdateUserDonationAvailabilityAsync(userId, donationAvailabilityId);
        //        return Ok(new { success = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = "failed", message = ex.Message });
        //    }
        //}
        /// <summary>
        /// Send welcome email to new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("send-welcome-email")]
        public IActionResult SendWelcomeEmail([FromBody] WelcomeEmailRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { status = "failed", message = "Email is required" });
                }

                _userServices.SendWelcomeEmail(request.Email, request.UserName);
                return Ok(new { status = "success", message = "Welcome email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "failed", message = "Error sending welcome email", error = ex.Message });
            }
        }

        [HttpGet("{userId}/image")]
        public async Task<IActionResult> GetUserImage(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { status = "failed", message = "Invalid user ID" });
            }

            try
            {
                var imageData = await _userServices.GetUserImageAsync(userId);

                if (imageData == null || imageData.Length == 0)
                {
                    return NotFound(new { status = "failed", message = "User image not found" });
                }

                return File(imageData, "image/jpeg"); // Default to JPEG, could be enhanced to detect actual format
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "failed", message = "Error retrieving user image", error = ex.Message });
            }
        }

        [HttpDelete("{userId}/image")]
        public async Task<IActionResult> DeleteUserImage(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { status = "failed", message = "Invalid user ID" });
            }

            try
            {
                var result = await _userServices.DeleteUserImageAsync(userId);

                if (!result)
                {
                    return NotFound(new { status = "failed", message = "User not found or image already deleted" });
                }

                return Ok(new { status = "success", message = "User image deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "failed", message = "Error deleting user image", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }
            return NoContent();
        }
        /// <summary>
        /// Forgot password for user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userServices.ForgotPasswordAsync(request.Email);
            if (!result)
            {
                return BadRequest(new { message = "Email sai hoặc không tồn tại" });
            }

            return Ok(new { message = "Nếu tài khoản của bạn tồn tại, một email hướng dẫn đặt lại mật khẩu đã được gửi." });
        }
        /// <summary>
        /// Reset password for user using token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userServices.ResetPasswordAsync(request.Token, request.NewPassword);

            if (!result)
            {
                return BadRequest(new { message = "Token không hợp lệ hoặc đã hết hạn." });
            }

            return Ok(new { message = "Mật khẩu của bạn đã được đặt lại thành công." });
        }
        /// <summary>
        /// Change password for user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Lấy UserID từ claims của JWT token
            var userIdString = User.FindFirst("UserId")?.Value;
            if(string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng." });
            }
            try
            {
                var result = await _userServices.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

                if (result)
                {
                    return Ok(new { message = "Đổi mật khẩu thành công." });
                }

                // Trường hợp này hiếm khi xảy ra nếu logic ở trên đúng
                return BadRequest(new { message = "Không thể đổi mật khẩu." });
            }
            catch (InvalidOperationException ex)
            {
                // Bắt lỗi cụ thể từ Service để trả về thông báo rõ ràng
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Các lỗi không mong muốn khác
                return StatusCode(500, new { message = "Đã có lỗi xảy ra.", error = ex.Message });
            }
        }
    }
}

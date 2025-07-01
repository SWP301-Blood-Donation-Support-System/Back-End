using BusinessLayer.IService;
using BusinessLayer.Utils;
using DataAccessLayer.DTO;
using Google.Apis.Auth;
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
        [HttpPatch("{userId}/role")]
        public async Task<IActionResult> SwitchRole(int userId,[FromBody] int roleId)
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
        [HttpPut("donor/{donorId}")]
        public async Task<IActionResult> UpdateDonor(int donorId, [FromForm] DonorDTO donorDTO)
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
    }
}

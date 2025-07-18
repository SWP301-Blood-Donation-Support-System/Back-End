using BusinessLayer.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyBloodEmailController : ControllerBase
    {
        private readonly IEmergencyBloodEmailService _emergencyBloodEmailService;

        public EmergencyBloodEmailController(IEmergencyBloodEmailService emergencyBloodEmailService)
        {
            _emergencyBloodEmailService = emergencyBloodEmailService;
        }

        /// <summary>
        /// Send emergency blood request email to all compatible donors
        /// </summary>
        /// <param name="bloodRequestId">ID of the blood request</param>
        /// <returns></returns>
        [HttpPost("send-emergency/{bloodRequestId}")]
        public async Task<IActionResult> SendEmergencyBloodRequestEmail(int bloodRequestId)
        {
            try
            {
                if (bloodRequestId <= 0)
                {
                    return BadRequest(new
                    {
                        status = "failed",
                        message = "Invalid blood request ID"
                    });
                }

                await _emergencyBloodEmailService.SendEmergencyBloodRequestEmailAsync(bloodRequestId);

                return Ok(new
                {
                    status = "success",
                    message = "Emergency blood request emails sent successfully to compatible donors",
                    bloodRequestId = bloodRequestId
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "failed", 
                    message = "An error occurred while sending emergency blood request emails",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Send emergency blood request email to donors with specific blood type and component compatibility
        /// </summary>
        /// <param name="bloodRequestId">ID of the blood request</param>
        /// <param name="bloodTypeId">Blood type ID needed</param>
        /// <param name="componentId">Blood component ID needed</param>
        /// <returns></returns>
        [HttpPost("send-emergency/{bloodRequestId}/type/{bloodTypeId}/component/{componentId}")]
        public async Task<IActionResult> SendEmergencyBloodRequestToCompatibleDonors(
            int bloodRequestId, 
            int bloodTypeId, 
            int componentId)
        {
            try
            {
                if (bloodRequestId <= 0 || bloodTypeId <= 0 || componentId <= 0)
                {
                    return BadRequest(new
                    {
                        status = "failed",
                        message = "Invalid blood request ID, blood type ID, or component ID"
                    });
                }

                await _emergencyBloodEmailService.SendEmergencyBloodRequestToCompatibleDonorsAsync(
                    bloodRequestId, bloodTypeId, componentId);

                return Ok(new
                {
                    status = "success",
                    message = "Emergency blood request emails sent successfully to compatible donors",
                    bloodRequestId = bloodRequestId,
                    bloodTypeId = bloodTypeId,
                    componentId = componentId
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "failed",
                    message = "An error occurred while sending emergency blood request emails",
                    error = ex.Message
                });
            }
        }
    }
}
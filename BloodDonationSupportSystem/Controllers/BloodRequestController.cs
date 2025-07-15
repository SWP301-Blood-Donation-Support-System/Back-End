using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BloodRequestController : ControllerBase
    {
        private readonly IBloodRequestService _bloodRequestService;
        public BloodRequestController(IBloodRequestService bloodRequestService)
        {
            _bloodRequestService = bloodRequestService;
        }

        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin/Staff được xem tất cả
        [HttpGet]
        public async Task<IActionResult> GetAllBloodRequestsAsync()
        {
            var bloodRequests = await _bloodRequestService.GetAllBloodRequestsAsync();
            return Ok(bloodRequests);
        }
        /// <summary>
        /// Get blood request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBloodRequestByIdAsync(int id)
        {
            var bloodRequest = await _bloodRequestService.GetBloodRequestsByIdAsync(id);
            if (bloodRequest == null)
            {
                return NotFound();
            }
            return Ok(bloodRequest);
        }
        /// <summary>
        /// Get blood requests by status ID
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetBloodRequestsByStatusIdAsync(int statusId)
        {
            var bloodRequests = await _bloodRequestService.GetBloodRequestsByStatusIdAsync(statusId);
            if (bloodRequests == null || !bloodRequests.Any())
            {
                return NotFound("No blood requests found for the given status ID.");
            }
            return Ok(bloodRequests);
        }
        /// <summary>
        /// Get blood requests by urgency ID
        /// </summary>
        /// <param name="urgencyId"></param>
        /// <returns></returns>
        [HttpGet("urgency/{urgencyId}")]
        public async Task<IActionResult> GetBloodRequestsByUrgencyIdAsync(int urgencyId)
        {
            var bloodRequests = await _bloodRequestService.GetBloodRequestsByUrgencyIdAsync(urgencyId);
            if(bloodRequests==null || !bloodRequests.Any())
            {
                return NotFound("No blood requests found for the given urgency ID.");
            }
            return Ok(bloodRequests);
        }

        /// <summary>
        /// Create blood request
        /// </summary>
        /// <param name="bloodRequest"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = "Staff,Hospital")] // Staff và Hospital có thể tạo yêu cầu
        public async Task<IActionResult> AddBloodRequestAsync([FromBody] BloodRequestDTO bloodRequest)
        {
            if (bloodRequest == null)
            {
                return BadRequest("Invalid blood request data.");
            }
            await _bloodRequestService.AddBloodRequestAsync(bloodRequest);
            return Ok(bloodRequest);
        }

        [HttpPatch("{requestId}/status")]
        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin/Staff được cập nhật trạng thái
        public async Task<IActionResult> UpdateBloodRequestStatusAsync(int requestId, [FromBody] int statusId)
        {
            if (requestId <= 0 || statusId <= 0)
            {
                return BadRequest("Invalid request data.");
            }
            var result = await _bloodRequestService.UpdateBloodRequestStatusAsync(requestId, statusId);
            if (!result)
            {
                return NotFound("Blood request not found or status update failed.");
            }
            return Ok("Blood request status updated successfully.");
        }
        /// <summary>
        /// Approve blood request by requestId and approverUserId in the body
        /// Chỉ Admin/Staff được duyệt
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="approvedByUserId"></param>
        /// <returns></returns>
        [HttpPatch("{requestId}/approve")]
        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin/Staff được duyệt
        public async Task<IActionResult> ApproveBloodRequestAsync(int requestId, [FromBody] int approvedByUserId)
        {
            if (requestId <= 0 || approvedByUserId <= 0)
            {
                return BadRequest("Invalid request or user ID.");
            }
            var result = await _bloodRequestService.ApproveBloodRequestAsync(requestId, approvedByUserId);
            if (!result)
            {
                return NotFound("Blood request not found or approval failed.");
            }
            return Ok("Blood request approved successfully.");
        }
        /// <summary>
        /// Reject blood request by requestId and approverUserId in the body
        /// Chỉ Admin/Staff được từ chối
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="rejectDTO"></param>
        /// <returns></returns>
        [HttpPatch("{requestId}/reject")]
        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin/Staff được từ chối
        public async Task<IActionResult> RejectBloodRequestAsync(int requestId, [FromBody] BloodRequestRejectDTO rejectDTO)
        {
            if (requestId <= 0)
            {
                return BadRequest("Invalid request.");
            }
            var result = await _bloodRequestService.RejectBloodRequestAsync(requestId, rejectDTO.UserId, rejectDTO.Reason);
            if (!result)
            {
                return NotFound("Blood request not found or rejection failed.");
            }
            return Ok("Blood request rejected successfully.");
        }
        /// <summary>
        /// Auto calculate and give a list of blood unit that is most suited for assignment to this request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [HttpGet("{requestId}/suggested-blood-unit-list")]
        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin/Staff được xem gợi ý
        public async Task<IActionResult> AutoAssignBloodUnitsToRequests(int requestId)
        {
            try
            {
                if (requestId <= 0)
                {
                    return BadRequest("Invalid request ID.");
                }
                var result = await _bloodRequestService.AutoAssignBloodUnitsToRequestAsync(requestId);
                if (result == null)
                {
                    return NotFound("Blood request not found or auto-assignment failed.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }

        }
    }
}

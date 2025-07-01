using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodRequestController : ControllerBase
    {
        private readonly IBloodRequestService _bloodRequestService;
        public BloodRequestController(IBloodRequestService bloodRequestService)
        {
            _bloodRequestService = bloodRequestService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBloodRequestsAsync()
        {
            var bloodRequests = await _bloodRequestService.GetAllBloodRequestsAsync();
            return Ok(bloodRequests);
        }
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
        [HttpPost]
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
        public async Task<IActionResult> UpdateBloodRequestStatusAsync(int requestId, [FromBody] int statusId)
        {
            if (requestId<=0 || statusId <= 0)
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
        [HttpPatch("{requestId}/approve")]
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
        [HttpPatch("{requestId}/reject")]
        public async Task<IActionResult> RejectBloodRequestAsync(int requestId, [FromBody] BloodRequestRejectDTO rejectDTO)
        {
            if (requestId <= 0 )
            {
                return BadRequest("Invalid request.");
            }
            var result = await _bloodRequestService.RejectBloodRequestAsync(requestId, rejectDTO.UserId,rejectDTO.Reason );
            if (!result)
            {
                return NotFound("Blood request not found or rejection failed.");
            }
            return Ok("Blood request rejected successfully.");
        }

    }
}

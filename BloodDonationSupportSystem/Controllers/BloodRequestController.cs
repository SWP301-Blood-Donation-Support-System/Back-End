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

    }
}

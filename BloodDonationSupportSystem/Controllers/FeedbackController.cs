using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var records = await _feedbackService.GetAllFeedbacksAsync();
            return Ok(records);
        }

        [HttpGet("{feedId:int}")]
        public async Task<IActionResult> GetFeedbackById(int feedId)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(feedId);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }

        [HttpGet("registration/{registrationId:int}")]
        public async Task<IActionResult> GetFeedbackByRegistrationId(int registrationId)
        {
            var feedback = await _feedbackService.GetFeedbackByRegistrationIdAsync(registrationId);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackDTO feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _feedbackService.AddAsync(feedback);
            if (!result)
                return StatusCode(500, "An error occurred while saving feedback.");
            return Ok(new { message = "Feedback added successfully." });
        }

        [HttpPatch("{feedId:int}")]
        public async Task<IActionResult> SoftDeleteFeedback(int feedId)
        {
            var result = await _feedbackService.SoftDeleteFeedbackAsync(feedId);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}

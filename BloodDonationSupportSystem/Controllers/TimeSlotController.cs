using BusinessLayer.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotServices _timeSlotServices;
        public TimeSlotController(ITimeSlotServices timeSlotServices)
        {
            _timeSlotServices = timeSlotServices ?? throw new ArgumentNullException(nameof(timeSlotServices));
        }
        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots()
        {
            try
            {
                var timeSlots = await _timeSlotServices.GetAvailableTimeSlotsAsync();
                return Ok(timeSlots);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", msg = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeSlotById(int id)
        {
            try
            {
                var timeSlot = await _timeSlotServices.GetTimeSlotByIdAsync(id);
                if (timeSlot == null)
                {
                    return NotFound(new { status = "failed", msg = "Time slot not found" });
                }
                return Ok(timeSlot);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", msg = ex.Message });
            }
        }
    }
}

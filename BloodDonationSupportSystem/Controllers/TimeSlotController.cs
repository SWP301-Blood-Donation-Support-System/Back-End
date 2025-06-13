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
        public async Task<IActionResult> GetAllTimeSlots()
        {
            try
            {
                var timeSlots = await _timeSlotServices.GetAllTimeSlotsAsync();
                return Ok(timeSlots);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", msg = ex.Message });
            }
        }
    }
}

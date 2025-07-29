using BusinessLayer.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        }
        /// <summary>
        /// Dashboard summary
        /// </summary>
        /// <returns></returns>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var dashboardData = await _dashboardService.GetDashboardSummaryAsync();
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
        }
        [HttpGet("donor-statistics")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            try
            {
                var statistics = await _dashboardService.GetDonorStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
        }
        [HttpGet("blood-inventory-statistics")]
        public async Task<IActionResult> GetBloodInventoryStatisticsAsync()
        {
            try
            {
                var statistics = await _dashboardService.GetBloodInventoryAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
        }
        [HttpGet("donation-activity-statistics")]
        public async Task<IActionResult> GetDonationActivityStatisticsAsync()
        {
            try
            {
                var statistics = await _dashboardService.GetDonationActivityAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
        }
        [HttpGet("blood-request-statistics")]
        public async Task<IActionResult> GetBloodRequestsStatisticsAsync()
        {
            try
            {
                var statistics = await _dashboardService.GetBloodRequestsStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
        }
        [HttpGet("hospital-activity-statistics")]
        public async Task<IActionResult> GetHospitalActivityStatisticsAsync()
        {
            try
            {
                var statistics = await _dashboardService.GetHospitalActivityAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }
        }
        [HttpGet("system-health")]
        public async Task<IActionResult> GetSystemHealthAsync()
        {
            try
            {
                var healthStatus = await _dashboardService.GetSystemHealthAsync();
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = ex.Message
                });
            }

        }
    }
}

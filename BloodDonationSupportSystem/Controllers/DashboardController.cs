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
        [HttpGet("donor-statistics-by-date")]
        public async Task<IActionResult> GetDashboardStatisticsByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var statistics = await _dashboardService.GetDonorStatisticsAsync(startDate, endDate);
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

    }
}

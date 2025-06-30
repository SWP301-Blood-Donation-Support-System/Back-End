using BusinessLayer.IService;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalService _hospitalService;
        public HospitalController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService ?? throw new ArgumentNullException(nameof(hospitalService));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHospitalsAsync()
        {
            var hospitals = await _hospitalService.GetAllHospitalsAsync();
            return Ok(hospitals);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospitalByIdAsync(int id)
        {
            var hospital = await _hospitalService.GetHospitalByIdAsync(id);
            if (hospital == null)
            {
                return NotFound();
            }
            return Ok(hospital);
        }
        [HttpPost]
        public async Task<IActionResult> AddHospitalAsync([FromBody] HospitalDTO hospital)
        {
            if (hospital == null)
            {
                return BadRequest("Invalid hospital data.");
            }
            await _hospitalService.AddHospitalAsync(hospital);
            return Ok(hospital);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospitalAsync(int id, [FromBody] HospitalDTO hospital)
        {
            if (hospital == null || id <= 0)
            {
                return BadRequest("Invalid hospital data or ID.");
            }
            
            var result = await _hospitalService.UpdateHospitalAsync(id, hospital);
            if (!result)
            {
                return NotFound("Hospital not found or update failed.");
            }
            return Ok("Hospital updated successfully.");
        }
    }
}

using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationRecordController : ControllerBase
    {
       private readonly IDonationRecordService _donationRecordService;
        public DonationRecordController(IDonationRecordService donationRecordService)
        {
            _donationRecordService = donationRecordService ?? throw new ArgumentNullException(nameof(donationRecordService));
        }
        [HttpGet("{recordId}")]
        public async Task<IActionResult> GetRecordById(int recordId)
        {
            if (recordId <= 0)
            {
                return BadRequest("Invalid record ID.");
            }
            var record = await _donationRecordService.GetRecordsByIdAsync(recordId);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }
        // Additional methods for other endpoints can be added here
        [HttpGet]
        public async Task<IActionResult> GetAllRecords()
        {
            var records = await _donationRecordService.GetAllDonationRecordsAsync();
            return Ok(records);
        }
        [HttpPost]
        public async Task<IActionResult> AddRecord([FromBody] DonationRecordDTO donationRecord)
        {
            if (donationRecord == null)
            {
                return BadRequest("Donation record cannot be null.");
            }
            
            var addedRecord = await _donationRecordService.AddRecordsAsync(donationRecord);
            return CreatedAtAction(nameof(GetRecordById), new { recordId = addedRecord.DonationRecordId }, addedRecord);
        }
    }
}

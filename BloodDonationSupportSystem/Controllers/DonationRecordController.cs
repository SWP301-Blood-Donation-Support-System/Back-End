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
            try
            {
                await _donationRecordService.AddRecordsAsync(donationRecord);

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "failed",
                    msg = ex.Message
                });


            }
            return Ok("Record added");
        }
        
        [HttpPut("{recordId}")]
        public async Task<IActionResult> UpdateRecord(int recordId, [FromBody] DonationRecordUpdateDTO updateDTO)
        {
            if (recordId <= 0)
            {
                return BadRequest("Invalid record ID.");
            }

            if (recordId != updateDTO.RecordId)
            {
                return BadRequest("Record ID mismatch.");
            }

            try
            {
                var result = await _donationRecordService.UpdateRecordByFieldsAsync(updateDTO);
                if (result)
                {
                    return Ok(new { status = "success", message = "Record updated successfully." });
                }
                else
                {
                    return NotFound("Record not found or could not be updated.");
                }
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
        
        [HttpPost("{recordId}/validate")]
        public async Task<IActionResult> ValidateRecord(int recordId, [FromBody] DonationValidationDTO validationDTO)
        {
            if (recordId != validationDTO.DonationRecordId)
            {
                return BadRequest("Record ID mismatch");
            }

            var result = await _donationRecordService.ValidateDonationRecordAsync(recordId, validationDTO.UserId);
            if (result)
                return Ok();
            return BadRequest("Failed to validate record");
        }

        [HttpGet("{recordId}/validations")]
        public async Task<IActionResult> GetValidations(int recordId)
        {
            var validations = await _donationRecordService.GetRecordsByIdAsync(recordId); // Adjusted method call
            if (validations == null)
            {
                return NotFound("No validations found for the given record ID.");
            }
            return Ok(validations);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRecordsByUserId(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var records = await _donationRecordService.GetRecordsByUserId(userId);
            if (records == null || !records.Any())
            {
                return NotFound("No records found for the given user ID.");
            }
            return Ok(records);

        }
    }
}

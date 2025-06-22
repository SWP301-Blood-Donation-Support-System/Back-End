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

        [HttpPut("{recordId}")]
        public async Task<IActionResult> UpdateRecord(int recordId, [FromBody] DonationRecordUpdateDTO updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Update data cannot be null.");
            }

            if (recordId != updateDto.DonationRecordId)
            {
                return BadRequest("Record ID mismatch between URL and body.");
            }

            try
            {
                var result = await _donationRecordService.UpdateRecordsAsync(recordId, updateDto);
                if (!result)
                {
                    return NotFound($"Donation record with ID {recordId} not found.");
                }
                return Ok(new { status = "success", message = "Record updated successfully" });
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
        [HttpPut("record-status")]
        public async Task<IActionResult> UpdateRecordStatus([FromBody] UpdateRecordStatusDTO updateRecordStatusDTO)
        {
            if(updateRecordStatusDTO == null)
            {
                return BadRequest("Update data cannot be null");
            }
            try
            {
                var result = await _donationRecordService.UpdateRecordsStatusAsync(updateRecordStatusDTO.RecordId,updateRecordStatusDTO.StatusId);
                if (!result)
                {
                    return NotFound($"Donation record with ID {updateRecordStatusDTO.RecordId} not found.");
                }
                return Ok(new { status = "success", message = "Record updated successfully" });
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

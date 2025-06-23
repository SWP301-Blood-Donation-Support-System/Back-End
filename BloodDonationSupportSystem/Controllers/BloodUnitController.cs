using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodUnitController : ControllerBase
    {
        private readonly IBloodUnitService _bloodUnitService;
        public BloodUnitController(IBloodUnitService bloodUnitService)
        {
            _bloodUnitService = bloodUnitService;
        }
        [HttpGet("GetAllBloodUnits")]
        public async Task<IActionResult> GetAllBloodUnits()
        {
            var bloodUnits = await _bloodUnitService.GetAllBloodUnitsAsync();
            
            // Create anonymous object with donor information
            var result = bloodUnits.Select(bu => new
            {
                bu.BloodUnitId,
                bu.DonationRecordId,
                bu.BloodTypeId,
                BloodTypeName = bu.BloodType?.BloodTypeName,
                bu.ComponentId,
                ComponentName = bu.Component?.ComponentName,
                bu.CollectedDateTime,
                bu.ExpiryDateTime,
                bu.Volume,
                bu.BloodUnitStatusId,
                StatusName = bu.BloodUnitStatus?.StatusName,
                DonorId = bu.DonationRecord?.Registration?.DonorId,
                DonorName = bu.DonationRecord?.Registration?.Donor?.FullName,
                bu.CreatedAt,
                bu.UpdatedAt
            });
            
            return Ok(result);
        }
        [HttpGet("GetBloodUnitById/{id}")]
        public async Task<IActionResult> GetBloodUnitById(int id)
        {
            var bloodUnit = await _bloodUnitService.GetBloodUnitByIdAsync(id);
            if (bloodUnit == null)
            {
                return NotFound($"No blood unit found with ID {id}.");
            }
            
            // Create anonymous object with donor information
            var result = new
            {
                bloodUnit.BloodUnitId,
                bloodUnit.DonationRecordId,
                bloodUnit.BloodTypeId,
                BloodTypeName = bloodUnit.BloodType?.BloodTypeName,
                bloodUnit.ComponentId,
                ComponentName = bloodUnit.Component?.ComponentName,
                bloodUnit.CollectedDateTime,
                bloodUnit.ExpiryDateTime,
                bloodUnit.Volume,
                bloodUnit.BloodUnitStatusId,
                StatusName = bloodUnit.BloodUnitStatus?.StatusName,
                DonorId = bloodUnit.DonationRecord?.Registration?.DonorId,
                DonorName = bloodUnit.DonationRecord?.Registration?.Donor?.FullName,
                bloodUnit.CreatedAt,
                bloodUnit.UpdatedAt
            };
            
            return Ok(result);
        }
        [HttpPost("AddBloodUnit")]
        public async Task<IActionResult> AddBloodUnit([FromBody] BloodUnitDTO bloodUnitDTO)
        {
            try
            {
                await _bloodUnitService.AddBloodUnitAsync(bloodUnitDTO);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
            return Ok("Blood unit added successfully.");
        }
        [HttpGet("GetBloodUnitsByBloodType/{bloodTypeId}")]
        public async Task<IActionResult> GetBloodUnitsByBloodType(int bloodTypeId)
        {
            var bloodUnits = await _bloodUnitService.GetBloodUnitsByBloodTypeAsync(bloodTypeId);
            return Ok(bloodUnits);
        }
        [HttpGet("GetBloodUnitsByBloodComponent/{bloodComponentId}")]
        public async Task<IActionResult> GetBloodUnitsByBloodComponent(int bloodComponentId)
        {
            var bloodUnits = await _bloodUnitService.GetBloodUnitsByBloodComponentAsync(bloodComponentId);
            return Ok(bloodUnits);
        }
        [HttpGet("GetBloodUnitsByStatus/{statusId}")]
        public async Task<IActionResult> GetBloodUnitsByStatus(int statusId)
        {
            var bloodUnits = await _bloodUnitService.GetBloodUnitsByStatusAsync(statusId);
            return Ok(bloodUnits);
        }
        [HttpPut("UpdateBloodUnitStatus/{unitId}/{bloodUnitStatusId}")]
        public async Task<IActionResult> UpdateBloodUnitStatus(int unitId, int bloodUnitStatusId)
        {

            var result = await _bloodUnitService.UpdateBloodUnitStatusAsync(unitId, bloodUnitStatusId);
            if (!result)
            {
                return NotFound($"No blood unit found with ID {unitId}.");
            }
            return Ok("Blood unit status updated successfully.");
        }
        [HttpDelete("DeleteBloodUnit/{id}")]
        public async Task<IActionResult> DeleteBloodUnit(int id)
        {
            var result = await _bloodUnitService.DeleteBloodUnitAsync(id);
            if (!result)
            {
                return NotFound($"No blood unit found with ID {id}.");
            }
            return Ok("Blood unit deleted successfully.");
        }
        [HttpPut("UpdateBloodUnit")]
        public async Task<IActionResult> UpdateBloodUnit([FromBody] BloodUnit bloodUnit)
        {
            if (bloodUnit == null || bloodUnit.BloodUnitId <= 0)
            {
                return BadRequest("Invalid blood unit data.");
            }
            var result = await _bloodUnitService.UpdateBloodUnitAsync(bloodUnit);
            if (!result)
            {
                return NotFound($"No blood unit found with ID {bloodUnit.BloodUnitId}.");
            }
            return Ok("Blood unit updated successfully.");
        }
    }
}

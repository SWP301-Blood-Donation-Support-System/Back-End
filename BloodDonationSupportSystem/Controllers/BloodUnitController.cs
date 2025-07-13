using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authorization;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin và Staff có quyền truy cập
    public class BloodUnitController : ControllerBase
    {
        private readonly IBloodUnitService _bloodUnitService;
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IMapper _mapper;
        public BloodUnitController(IBloodUnitService bloodUnitService, IBloodUnitRepository bloodUnitRepository, IMapper mapper)
        {
            _bloodUnitService = bloodUnitService;
            _bloodUnitRepository = bloodUnitRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBloodUnits()
        {
            try
            {
                // Sử dụng ProjectTo để EF Core tạo câu lệnh SQL tối ưu
                var bloodUnitDTOs = await _bloodUnitRepository.GetAllAsQueryable()
                    .ProjectTo<BloodUnitResponseDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Ok(bloodUnitDTOs);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBloodUnitById(int id)
        {
            try
            {
                var bloodUnit = await _bloodUnitService.GetBloodUnitByIdAsync(id);
                if (bloodUnit == null)
                {
                    return NotFound($"No blood unit found with ID {id}.");
                }

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
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        [HttpPost]
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
        [HttpGet("by-blood-type/{bloodTypeId}")]
        public async Task<IActionResult> GetBloodUnitsByBloodType(int bloodTypeId)
        {
            try
            {
                var bloodUnits = await _bloodUnitService.GetBloodUnitsByBloodTypeAsync(bloodTypeId);

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
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        [HttpGet("by-blood-component/{bloodComponentId}")]
        public async Task<IActionResult> GetBloodUnitsByBloodComponent(int bloodComponentId)
        {
            try
            {
                var bloodUnits = await _bloodUnitService.GetBloodUnitsByBloodComponentAsync(bloodComponentId);

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
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        [HttpGet("by-status/{statusId}")]
        public async Task<IActionResult> GetBloodUnitsByStatus(int statusId)
        {
            try
            {
                var bloodUnits = await _bloodUnitService.GetBloodUnitsByStatusAsync(statusId);

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
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        [HttpPatch("{unitId}/status")]
        public async Task<IActionResult> UpdateBloodUnitStatus(int unitId, [FromBody] int statusId)
        {
            try
            {
                // 2. Validation tự động: 
                // Nhờ [ApiController] và các DataAnnotations ([Required], [Range]) trong model,
                // nếu client gửi UnitId <= 0, request sẽ tự động bị từ chối với lỗi 400 Bad Request.
                // Anh không cần viết code "if (id <= 0)" nữa.

                // 3. Lấy dữ liệu từ request body
                var result = await _bloodUnitService.UpdateBloodUnitStatusAsync(unitId, statusId);

                if (!result)
                {
                    return NotFound($"No blood unit found with ID {unitId}.");
                }

                return Ok("Blood unit status updated successfully.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBloodUnit(int id)
        {
            try
            {
                var result = await _bloodUnitService.DeleteBloodUnitAsync(id);
                if (!result)
                {
                    return NotFound($"No blood unit found with ID {id}.");
                }
                return Ok("Blood unit deleted successfully.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBloodUnit([FromBody] BloodUnit bloodUnit)
        {
            try
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
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
        /// <summary>
        /// Assign blood unit to a request using unitId and requestId in the body
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [HttpPatch("{unitId}/assign-to-request")]
        public async Task<IActionResult> AssignBloodUnitToRequest(int unitId, [FromBody] int requestId)
        {
            try
            {
                if (unitId <= 0 || requestId <= 0)
                {
                    return BadRequest("Invalid unit ID or request ID.");
                }
                var result = await _bloodUnitService.AssignBloodUnitToRequestAsync(unitId, requestId);
                if (!result)
                {
                    return NotFound($"No blood unit found with ID {unitId} or failed to assign to request {requestId}.");
                }
                return Ok("Blood unit assigned to request successfully.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });
            }
        }
    }
}

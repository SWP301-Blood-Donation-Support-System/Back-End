using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationScheduleController : ControllerBase
    {
        private readonly IDonationScheduleService _donationScheduleService;
        private readonly IMapper _mapper;

        public DonationScheduleController(IDonationScheduleService donationScheduleService, IMapper mapper)
        {
            _donationScheduleService = donationScheduleService ?? 
                throw new ArgumentNullException(nameof(donationScheduleService));
            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSchedules()
        {
            try
            {
                var schedules = await _donationScheduleService.GetAllDonationSchedulesAsync();
                var scheduleDTOs = _mapper.Map<IEnumerable<DonationScheduleDTO>>(schedules);
                return Ok(scheduleDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "An error occurred while retrieving schedules", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid schedule ID.");
            }

            try
            {
                var schedule = await _donationScheduleService.GetDonationScheduleByIdAsync(id);
                if (schedule == null)
                {
                    return NotFound($"No schedule found with ID {id}.");
                }

                var scheduleDTO = _mapper.Map<DonationScheduleDTO>(schedule);
                return Ok(scheduleDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"An error occurred while retrieving schedule {id}", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] DonationScheduleDTO scheduleDTO)
        {
            if (scheduleDTO == null)
            {
                return BadRequest("Schedule data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Ensure required date is provided
                if (!scheduleDTO.ScheduleDate.HasValue)
                {
                    return BadRequest("Schedule date is required.");
                }

                var schedule = _mapper.Map<DonationSchedule>(scheduleDTO);
                string createdBy = User.Identity?.Name ?? "System";
                
                var createdSchedule = await _donationScheduleService.CreateDonationScheduleAsync(schedule, createdBy);
                
                var createdScheduleDTO = _mapper.Map<DonationScheduleDTO>(createdSchedule);
                return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.ScheduleId }, createdScheduleDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] DonationScheduleDTO scheduleDTO)
        {
            if (scheduleDTO == null)
            {
                return BadRequest("Schedule data is required.");
            }

            if (id <= 0)
            {
                return BadRequest("Invalid schedule ID.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get existing schedule to preserve other properties
                var existingSchedule = await _donationScheduleService.GetDonationScheduleByIdAsync(id);
                if (existingSchedule == null)
                {
                    return NotFound($"No schedule found with ID {id}.");
                }
                
                // Update only the ScheduleDate property from the DTO
                existingSchedule.ScheduleDate = scheduleDTO.ScheduleDate;
                
                string updatedBy = User.Identity?.Name ?? "System";
                var success = await _donationScheduleService.UpdateDonationScheduleAsync(existingSchedule, updatedBy);

                if (!success)
                {
                    return NotFound($"No schedule found with ID {id}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid schedule ID.");
            }

            try
            {
                string deletedBy = User.Identity?.Name ?? "System";
                
                var success = await _donationScheduleService.SoftDeleteDonationScheduleAsync(id, deletedBy);

                if (!success)
                {
                    return NotFound($"No schedule found with ID {id}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingSchedules()
        {
            try
            {
                var schedules = await _donationScheduleService.GetUpcomingAvailableDonationSchedulesAsync();
                var scheduleDTOs = _mapper.Map<IEnumerable<DonationScheduleDTO>>(schedules);
                return Ok(scheduleDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "An error occurred while retrieving upcoming schedules", details = ex.Message });
            }
        }

        [HttpGet("{id}/with-registrations")]
        public async Task<IActionResult> GetScheduleWithRegistrations(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid schedule ID.");
            }

            try
            {
                var schedule = await _donationScheduleService.GetDonationScheduleWithRegistrationsAndDetailsAsync(id);
                if (schedule == null)
                {
                    return NotFound($"No schedule found with ID {id}.");
                }

                // Just return the simplified DTO with schedule date
                var scheduleDTO = _mapper.Map<DonationScheduleDTO>(schedule);
                return Ok(scheduleDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"An error occurred while retrieving schedule {id} with registrations", details = ex.Message });
            }
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreSchedule(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid schedule ID.");
            }

            try
            {
                string restoredBy = User.Identity?.Name ?? "System";
                
                var success = await _donationScheduleService.RestoreDonationScheduleAsync(id, restoredBy);

                if (!success)
                {
                    return NotFound($"No deleted schedule found with ID {id}.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
        }
    }
}
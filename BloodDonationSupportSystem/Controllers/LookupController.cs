using BusinessLayer.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        [HttpGet("article-categories")]
        public async Task<IActionResult> GetArticleCategories()
        {
            try
            {
                var categories = await _lookupService.GetArticleCategoriesAsync();
                return Ok(categories);
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

        [HttpGet("article-statuses")]
        public async Task<IActionResult> GetArticleStatuses()
        {
            try
            {
                var statuses = await _lookupService.GetArticleStatusesAsync();
                return Ok(statuses);
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

        [HttpGet("blood-components")]
        public async Task<IActionResult> GetBloodComponents()
        {
            try
            {
                var components = await _lookupService.GetBloodComponentsAsync();
                return Ok(components);
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

        [HttpGet("blood-request-statuses")]
        public async Task<IActionResult> GetBloodRequestStatuses()
        {
            try
            {
                var statuses = await _lookupService.GetBloodRequestStatusesAsync();
                return Ok(statuses);
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

        [HttpGet("blood-test-results")]
        public async Task<IActionResult> GetBloodTestResults()
        {
            try
            {
                var results = await _lookupService.GetBloodTestResultsAsync();
                return Ok(results);
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

        [HttpGet("blood-types")]
        public async Task<IActionResult> GetBloodTypes()
        {
            try
            {
                var types = await _lookupService.GetBloodTypesAsync();
                return Ok(types);
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

        [HttpGet("blood-unit-statuses")]
        public async Task<IActionResult> GetBloodUnitStatuses()
        {
            try
            {
                var statuses = await _lookupService.GetBloodUnitStatusesAsync();
                return Ok(statuses);
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

        [HttpGet("donation-availabilities")]
        public async Task<IActionResult> GetDonationAvailabilities()
        {
            try
            {
                var availabilities = await _lookupService.GetDonationAvailabilitiesAsync();
                return Ok(availabilities);
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

        [HttpGet("donation-types")]
        public async Task<IActionResult> GetDonationTypes()
        {
            try
            {
                var types = await _lookupService.GetDonationTypesAsync();
                return Ok(types);
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

        [HttpGet("genders")]
        public async Task<IActionResult> GetGenders()
        {
            try
            {
                var genders = await _lookupService.GetGendersAsync();
                return Ok(genders);
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

        [HttpGet("notification-types")]
        public async Task<IActionResult> GetNotificationTypes()
        {
            try
            {
                var types = await _lookupService.GetNotificationTypesAsync();
                return Ok(types);
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

        [HttpGet("occupations")]
        public async Task<IActionResult> GetOccupations()
        {
            try
            {
                var occupations = await _lookupService.GetOccupationsAsync();
                return Ok(occupations);
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

        [HttpGet("registration-statuses")]
        public async Task<IActionResult> GetRegistrationStatuses()
        {
            try
            {
                var statuses = await _lookupService.GetRegistrationStatusesAsync();
                return Ok(statuses);
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

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _lookupService.GetRolesAsync();
                return Ok(roles);
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


        [HttpGet("urgencies")]
        public async Task<IActionResult> GetUrgencies()
        {
            try
            {
                var urgencies = await _lookupService.GetUrgenciesAsync();
                return Ok(urgencies);
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

        // Convenience method to get all lookup data at once
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLookupData()
        {
            try
            {
                var result = new
                {
                    ArticleCategories = await _lookupService.GetArticleCategoriesAsync(),
                    ArticleStatuses = await _lookupService.GetArticleStatusesAsync(),
                    BloodComponents = await _lookupService.GetBloodComponentsAsync(),
                    BloodRequestStatuses = await _lookupService.GetBloodRequestStatusesAsync(),
                    BloodTestResults = await _lookupService.GetBloodTestResultsAsync(),
                    BloodTypes = await _lookupService.GetBloodTypesAsync(),
                    BloodUnitStatuses = await _lookupService.GetBloodUnitStatusesAsync(),
                    DonationAvailabilities = await _lookupService.GetDonationAvailabilitiesAsync(),
                    DonationTypes = await _lookupService.GetDonationTypesAsync(),
                    Genders = await _lookupService.GetGendersAsync(),
                    NotificationTypes = await _lookupService.GetNotificationTypesAsync(),
                    Occupations = await _lookupService.GetOccupationsAsync(),
                    RegistrationStatuses = await _lookupService.GetRegistrationStatusesAsync(),
                    Roles = await _lookupService.GetRolesAsync(),
                    Urgencies = await _lookupService.GetUrgenciesAsync()
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
    }
}
using DataAccessLayer.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface ILookupService
    {
        Task<IEnumerable<LookupDTO>> GetArticleCategoriesAsync();
        Task<IEnumerable<LookupDTO>> GetArticleStatusesAsync();
        Task<IEnumerable<LookupDTO>> GetBloodComponentsAsync();
        Task<IEnumerable<LookupDTO>> GetBloodRequestStatusesAsync();
        Task<IEnumerable<LookupDTO>> GetBloodTestResultsAsync();
        Task<IEnumerable<LookupDTO>> GetBloodTypesAsync();
        Task<IEnumerable<LookupDTO>> GetBloodUnitStatusesAsync();
        Task<IEnumerable<LookupDTO>> GetDonationAvailabilitiesAsync();
        Task<IEnumerable<LookupDTO>> GetDonationTypesAsync();
        Task<IEnumerable<LookupDTO>> GetGendersAsync();
        Task<IEnumerable<LookupDTO>> GetNotificationTypesAsync();
        Task<IEnumerable<LookupDTO>> GetOccupationsAsync();
        Task<IEnumerable<LookupDTO>> GetRegistrationStatusesAsync();
        Task<IEnumerable<LookupDTO>> GetRolesAsync();
        Task<IEnumerable<LookupDTO>> GetUrgenciesAsync();
    }
}
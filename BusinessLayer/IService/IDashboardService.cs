using DataAccessLayer.DTO.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDTO> GetDashboardSummaryAsync();
        Task<DonorStatisticsDTO> GetDonorStatisticsAsync();
        Task<DonorStatisticsDTO> GetDonorStatisticsAsync(DateTime startDate, DateTime endDate);
        Task<BloodInventoryDTO> GetBloodInventoryAsync();
        Task<BloodInventoryDTO> GetBloodInventoryAsync(DateTime startDate, DateTime endDate);
        Task<DonationActivityDTO> GetDonationActivityAsync();
        Task<DonationActivityDTO> GetDonationActivityAsync(DateTime startDate, DateTime endDate);
        Task<BloodRequestsDTO> GetBloodRequestsAsync();
        Task<BloodRequestsDTO> GetBloodRequestsAsync(DateTime startDate, DateTime endDate);
        Task<HospitalActivityDTO> GetHospitalActivityAsync();
        Task<HospitalActivityDTO> GetHospitalActivityAsync(DateTime startDate, DateTime endDate);
        Task<SystemHealthDTO> GetSystemHealthAsync();
    }
}

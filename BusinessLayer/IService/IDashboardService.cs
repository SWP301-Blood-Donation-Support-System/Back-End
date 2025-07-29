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
        Task<BloodInventoryDTO> GetBloodInventoryAsync();
        Task<DonationActivityDTO> GetDonationActivityAsync();
        Task<BloodRequestsDTO> GetBloodRequestsStatisticsAsync();
        Task<HospitalActivityDTO> GetHospitalActivityAsync();
        Task<SystemHealthDTO> GetSystemHealthAsync();
    }
}

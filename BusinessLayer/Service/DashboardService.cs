using BusinessLayer.IService;
using DataAccessLayer.DTO.Dashboard;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IDonationRegistrationRepository _donationRegistrationRepository;
        private readonly IDonationRecordRepository _donationRecordRepository;
        private readonly IHospitalRepository _hospitalRepository;
        
        public DashboardService(
            IUserRepository userRepository,
            IBloodUnitRepository bloodUnitRepository,
            IBloodRequestRepository bloodRequestRepository,
            IDonationRegistrationRepository donationRegistrationRepository,
            IDonationRecordRepository donationRecordRepository,
            IHospitalRepository hospitalRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _bloodUnitRepository = bloodUnitRepository ?? throw new ArgumentNullException(nameof(bloodUnitRepository));
            _bloodRequestRepository = bloodRequestRepository ?? throw new ArgumentNullException(nameof(bloodRequestRepository));
            _donationRegistrationRepository = donationRegistrationRepository ?? throw new ArgumentNullException(nameof(donationRegistrationRepository));
            _donationRecordRepository = donationRecordRepository ?? throw new ArgumentNullException(nameof(donationRecordRepository));
            _hospitalRepository = hospitalRepository ?? throw new ArgumentNullException(nameof(hospitalRepository));
        }
        
        public async Task<DashboardSummaryDTO> GetDashboardSummaryAsync()
        {
            try
            {
                var donorRoleId = 3; 
                var donors = await _userRepository.GetByRoleIdAsync(donorRoleId);
                var eligibleDonors = await _userRepository.GetEligibleDonorsAsync();
                
                var bloodUnits = _bloodUnitRepository.GetAllAsQueryable();
                var availableUnitStatus = 1; 
                var availableUnits = await bloodUnits.CountAsync(u => u.BloodUnitStatusId == availableUnitStatus);
                var availableVolume = await bloodUnits
                    .Where(u => u.BloodUnitStatusId == availableUnitStatus)
                    .SumAsync(u => u.Volume);
                
                var pendingRequestStatus = 2; // Status ID 2 for approved requests
                var requests = await _bloodRequestRepository.GetBloodRequestsByStatusIdAsync(pendingRequestStatus);
                
                // Donation metrics
                var scheduledRegistrationStatus = 1; // Status ID 1 for scheduled registrations
                var scheduledDonations = await _donationRegistrationRepository.GetRegistrationsByStatusIdAsync(scheduledRegistrationStatus);
                
                var approvedTestResult = 2; // Assuming 2 is "approved" result
                var donationRecords = await _donationRecordRepository.GetAllAsync();
                var completedDonationsThisMonth = donationRecords
                    .Where(r => r.DonationDateTime >= DateTime.Now.AddMonths(-1) && r.BloodTestResult == approvedTestResult)
                    .Count();
                
                // Fulfillment calculation
                var totalRequests = await _bloodRequestRepository.GetAllAsync();
                var completedRequests = totalRequests.Where(r => r.RequestStatusId == 3); // 3 is "completed"
                
                
                return new DashboardSummaryDTO
                {
                    TotalDonors = donors.Count(),
                    EligibleDonors = eligibleDonors.Count(),
                    AvailableBloodUnits = availableUnits,
                    PendingRequests = requests.Count(),
                    ScheduledDonations = scheduledDonations.Count(),
                    CompletedDonationsThisMonth = completedDonationsThisMonth,
                    TotalBloodVolumeAvailable = availableVolume,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting dashboard summary: {ex.Message}");
                throw;
            }
        }
        
        
        public async Task<DonorStatisticsDTO> GetDonorStatisticsAsync()
        {
            try
            {
                var donorRoleId = 3; // Role ID for donors
                var donors = await _userRepository.GetByRoleIdAsync(donorRoleId);
                var eligibleDonors = await _userRepository.GetEligibleDonorsAsync();
                var newDonorsThisMonth = donors.Where(d => d.CreatedAt >= DateTime.Now.AddMonths(-1)).Count();
                
                // Group donors by blood type
                var donorsByBloodType = donors
                    .Where(d => d.BloodTypeId.HasValue)
                    .GroupBy(d => d.BloodTypeId.Value)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Group donors by availability
                var donorsByAvailability = donors
                    .GroupBy(d => d.DonationAvailabilityId)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                return new DonorStatisticsDTO
                {
                    TotalDonors = donors.Count(),
                    EligibleDonors = eligibleDonors.Count(),
                    NewDonorsThisMonth = newDonorsThisMonth,
                    DonorsByBloodType = donorsByBloodType,
                    DonorsByAvailability = donorsByAvailability,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting donor statistics: {ex.Message}");
                throw;
            }
        }
        
        
        public async Task<BloodInventoryDTO> GetBloodInventoryAsync()
        {
            try
            {
                var bloodUnits = await _bloodUnitRepository.GetAllAsync();
                var availableStatus = 1; 
                var assignedStatus = 2;
                var expiredStatus = 3;
                var unusableStatus = 4;


                var availableUnits = bloodUnits.Where(u => u.BloodUnitStatusId == availableStatus).Count();
                var assignedUnits = bloodUnits.Where(u => u.BloodUnitStatusId == assignedStatus).Count();
                var unusableUnits = bloodUnits.Where(u => u.BloodUnitStatusId == unusableStatus).Count();
                var expiredUnits = bloodUnits.Where(u => u.BloodUnitStatusId == expiredStatus).Count();
                var expiringWithinWeek = bloodUnits
                    .Count(u => u.BloodUnitStatusId == availableStatus && 
                             u.ExpiryDateTime <= DateTime.Now.AddDays(7));
                
                // Group by blood type
                var unitsByBloodType = bloodUnits
                    .Where(u => u.BloodUnitStatusId == availableStatus)
                    .GroupBy(u => u.BloodTypeId)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Group by component
                var unitsByComponent = bloodUnits
                    .Where(u => u.BloodUnitStatusId == availableStatus)
                    .GroupBy(u => u.ComponentId)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Group by status
                var unitsByStatus = bloodUnits
                    .GroupBy(u => u.BloodUnitStatusId)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Get collection trend data
              
                
                return new BloodInventoryDTO
                {
                    TotalUnits = bloodUnits.Count(),
                    AvailableUnits = availableUnits,
                    AssignedUnits = assignedUnits,
                    ExpiredUnits = expiredUnits,
                    UnusableUnits=unusableUnits,
                    ExpiringWithinWeek = expiringWithinWeek,
                    UnitsByBloodType = unitsByBloodType,
                    UnitsByComponent = unitsByComponent,
                    UnitsByStatus = unitsByStatus,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting blood inventory: {ex.Message}");
                throw;
            }
        }
        
        
        public async Task<DonationActivityDTO> GetDonationActivityAsync()
        {
            try
            {
                var records = await _donationRecordRepository.GetAllRecordsWithDonor();
                var registrations = await _donationRegistrationRepository.GetAllAsync();
                
                var totalDonations = records.Count();
                var successfulTestResult = 2; // Assuming 2 is "approved"
                var completedDonations = records.Count(r => r.BloodTestResult == successfulTestResult);
                
                var scheduledRegistrationStatus = 1; // Assuming 1 is "scheduled"
                var scheduledDonations = registrations.Count(r => r.RegistrationStatusId == scheduledRegistrationStatus);
                
                // Calculate success rate
                decimal successRate = totalDonations > 0 
                    ? (decimal)completedDonations / totalDonations * 100 
                    : 0;
                
                // Calculate total volume collected
                var totalVolumeCollected = records
                    .Where(r => r.BloodTestResult == successfulTestResult)
                    .Sum(r => r.VolumeDonated);
                
                // Get recent donations
                var recentDonations = records
                    .Where(re=>re.BloodTestResult == successfulTestResult)
                    .OrderByDescending(r => r.DonationDateTime)
                    .Take(5)
                    .Select(r => new RecentDonationDTO
                    {
                        RecordId = r.DonationRecordId,
                        DonorName = r.Registration?.Donor?.FullName ?? "Unknown",
                        DonationDateTime = r.DonationDateTime,
                        Volume = r.VolumeDonated,
                        BloodTypeName = r.Registration?.Donor?.BloodType?.BloodTypeName ?? "Unknown",
                        DonationTypeId = r.DonationTypeId,
                        DonationTypeName = r.DonationType?.TypeName ?? "Unknown"
                    })
                    .ToList();
                
               
                // Group donations by type
                var donationsByType = records
                    .Where(r => r.DonationTypeId.HasValue)
                    .GroupBy(r => r.DonationTypeId.Value)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                return new DonationActivityDTO
                {
                    TotalDonations = totalDonations,
                    CompletedDonations = completedDonations,
                    ScheduledDonations = scheduledDonations,
                    SuccessRate = successRate,
                    TotalVolumeCollected = totalVolumeCollected,
                    RecentDonations = recentDonations,
                    DonationsByType = donationsByType
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting donation activity: {ex.Message}");
                throw;
            }
        }
        
        public async Task<BloodRequestsDTO> GetBloodRequestsStatisticsAsync()
        {
            try
            {
                var requests = await _bloodRequestRepository.GetAllAsync();
                
                var totalRequests = requests.Count();
                var pendingRequests = requests.Count(r => r.RequestStatusId == 1); 
                var approvedRequests = requests.Count(r => r.RequestStatusId == 2); 
                var completedRequests = requests.Count(r => r.RequestStatusId == 3); 
                
                var totalVolumeRequested = requests.Sum(r => r.Volume);
                var totalVolumeUnfulfilled = requests.Sum(r => r.RemainingVolume);
                
                decimal fulfillmentRate = 0;
                if (totalVolumeRequested > 0)
                {
                    fulfillmentRate = (totalVolumeRequested - totalVolumeUnfulfilled) / totalVolumeRequested * 100;
                }
                
                // Group by urgency
                var requestsByUrgency = requests
                    .Where(r => r.UrgencyId.HasValue)
                    .GroupBy(r => r.UrgencyId.Value)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Group by blood type
                var requestsByBloodType = requests
                    .GroupBy(r => r.BloodTypeId)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Get request trend data
              
                
                return new BloodRequestsDTO
                {
                    TotalRequests = totalRequests,
                    PendingRequests = pendingRequests,
                    ApprovedRequests = approvedRequests,
                    CompletedRequests = completedRequests,
                    TotalVolumeRequested = totalVolumeRequested,
                    TotalVolumeUnfulfilled = totalVolumeUnfulfilled,
                    FulfillmentRate = fulfillmentRate,
                    RequestsByUrgency = requestsByUrgency,
                    RequestsByBloodType = requestsByBloodType,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting blood requests: {ex.Message}");
                throw;
            }
        }
        
        public async Task<HospitalActivityDTO> GetHospitalActivityAsync()
        {
            try
            {
                var hospitals = await _hospitalRepository.GetAllAsync();
                var hospitalRoleId = 4; 
                var hospitalUsers = await _userRepository.GetByRoleIdAsync(hospitalRoleId);
                
                var activeHospitalCount = hospitalUsers.Count(h => h.IsActive);
                
                // Get all requests and group by hospital
                var requests = await _bloodRequestRepository.GetAllAsync();
                var requestsByHospital = requests
                    .Where(r => r.RequestingStaffId > 0)
                    .GroupBy(r => r.RequestingStaff?.HospitalId ?? 0)
                    .Where(g => g.Key > 0)
                    .ToDictionary(g => g.Key, g => g.ToList());
                
                // Get top requesting hospitals
                var topHospitals = requestsByHospital
                    .OrderByDescending(kvp => kvp.Value.Count)
                    .Take(5)
                    .Select(kvp => {
                        var hospital = hospitals.FirstOrDefault(h => h.HospitalId == kvp.Key);
                        return new HospitalRequestSummaryDTO
                        {
                            HospitalId = kvp.Key,
                            HospitalName = hospital?.HospitalName ?? "Unknown",
                            RequestCount = kvp.Value.Count,
                            TotalVolumeRequested = kvp.Value.Sum(r => r.Volume)
                        };
                    })
                    .ToList();
                
                // Calculate fulfillment rates for each hospital
                var hospitalFulfillment = new Dictionary<int, HospitalFulfillmentDTO>();
                
                foreach (var kvp in requestsByHospital)
                {
                    var hospitalId = kvp.Key;
                    var hospitalRequests = kvp.Value;
                    var hospital = hospitals.FirstOrDefault(h => h.HospitalId == hospitalId);
                    
                    var totalRequests = hospitalRequests.Count;
                    var fullyFulfilled = hospitalRequests.Count(r => r.RequestStatusId == 3); // Completed status
                    var unfulfilled = hospitalRequests.Count(r => r.RequestStatusId == 2);

                    decimal fulfillmentRate = totalRequests > 0 
                        ? (decimal)(fullyFulfilled) / totalRequests * 100 
                        : 0;
                    
                    hospitalFulfillment[hospitalId] = new HospitalFulfillmentDTO
                    {
                        HospitalId = hospitalId,
                        HospitalName = hospital?.HospitalName ?? "Unknown",
                        TotalRequests = totalRequests,
                        FulfilledRequests = fullyFulfilled,
                        UnfulfilledRequests = unfulfilled,
                        FulfillmentRate = fulfillmentRate
                    };
                }
                
                return new HospitalActivityDTO
                {
                    TotalHospitals = hospitals.Count(),
                    ActiveHospitals = activeHospitalCount,
                    TopRequestingHospitals = topHospitals,
                    HospitalFulfillmentRates = hospitalFulfillment
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting hospital activity: {ex.Message}");
                throw;
            }
        }
        
        public async Task<SystemHealthDTO> GetSystemHealthAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                
                // Group users by role
                var usersByRole = users
                    .GroupBy(u => u.RoleId)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Calculate active users in last 30 days
                var activeUsersLast30Days = users.Count(u => 
                    u.UpdatedAt.HasValue && u.UpdatedAt >= DateTime.Now.AddDays(-30));
                
                // Calculate registration to completion rate
                var donorRoleId = 3; 
                var donors = users.Where(u => u.RoleId == donorRoleId).ToList();
                
                var records = await _donationRecordRepository.GetAllAsync();
                var successfulTestResult = 2; 
                
                var totalRegisteredDonors = donors.Count;
                var donorsWithSuccessfulDonations = records
                    .Where(r => r.BloodTestResult == successfulTestResult)
                    .Select(r => r.Registration?.DonorId)
                    .Distinct()
                    .Count();
                
                var bloodUnits = await _bloodUnitRepository.GetAllAsync();
                
                return new SystemHealthDTO
                {
                    UsersByRole = usersByRole,
                    ActiveUsersLast30Days = activeUsersLast30Days,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting system health: {ex.Message}");
                throw;
            }
        }
    }
}
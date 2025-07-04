using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IBloodCompatibilityService _bloodCompatibilityService;
        private readonly IMapper _mapper;

        public BloodRequestService(
            IBloodRequestRepository bloodRequestRepository,
            IBloodUnitRepository bloodUnitRepository,
            IBloodCompatibilityService bloodCompatibilityService,
            IMapper mapper)
        {
            _bloodRequestRepository = bloodRequestRepository;
            _bloodUnitRepository = bloodUnitRepository;
            _bloodCompatibilityService = bloodCompatibilityService;
            _mapper = mapper;
        }

        public async Task AddBloodRequestAsync(BloodRequestDTO bloodRequest)
        {
            try
            {
                var entity = _mapper.Map<BloodRequest>(bloodRequest);
                entity.RequestStatusId = 1;
                entity.RequestDateTime = DateTime.UtcNow;
                await _bloodRequestRepository.AddAsync(entity);
                await _bloodRequestRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding blood request", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequest>> GetAllBloodRequestsAsync()
        {
            try
            {
                return await _bloodRequestRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving all blood requests", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByBloodTypeIdAsync(int bloodTypeId)
        {
            try
            {
                return await _bloodRequestRepository.GetBloodRequestsByBloodTypeIdAsync(bloodTypeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by blood type ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByComponentIdAsync(int componentId)
        {
            try
            {
                return await _bloodRequestRepository.GetBloodRequestsByComponentIdAsync(componentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by component ID", ex);
                throw;
            }
        }

        public async Task<BloodRequest> GetBloodRequestsByIdAsync(int id)
        {
            try
            {
                return await _bloodRequestRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood request by ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByStaffIdAsync(int staffId)
        {
            try
            {
                return await _bloodRequestRepository.GetBloodRequestsByStaffIdAsync(staffId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by staff ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByStatusIdAsync(int statusId)
        {
            try
            {
                return await _bloodRequestRepository.GetBloodRequestsByStatusIdAsync(statusId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by status ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByUrgencyIdAsync(int urgencyId)
        {
            try
            {
                return await _bloodRequestRepository.GetBloodRequestsByUrgencyIdAsync(urgencyId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by urgency ID", ex);
                throw;
            }
        }

        public async Task<bool> ApproveBloodRequestAsync(int requestId, int approvedByUserId)
        {
            try
            {
                var bloodRequest = await _bloodRequestRepository.GetByIdAsync(requestId);
                if (bloodRequest == null)
                {
                    throw new KeyNotFoundException("Blood request not found");
                }

                bloodRequest.ApprovedByUserId = approvedByUserId;
                bloodRequest.RequestStatusId = 2; // Approved status
                bloodRequest.UpdatedAt = DateTime.UtcNow;
                await _bloodRequestRepository.UpdateAsync(bloodRequest);
                await _bloodRequestRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error approving blood request", ex);
                throw;
            }
        }

        public async Task<bool> RejectBloodRequestAsync(int requestId, int rejectedByUserId, string? rejectReason)
        {
            try
            {
                var bloodRequest = await _bloodRequestRepository.GetByIdAsync(requestId);
                if (bloodRequest == null)
                {
                    throw new KeyNotFoundException("Blood request not found");
                }

                bloodRequest.ApprovedByUserId = rejectedByUserId;
                bloodRequest.RequestStatusId = 3;

                var currentNote = bloodRequest.Note ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(rejectReason))
                {
                    if (!string.IsNullOrWhiteSpace(currentNote))
                    {
                        currentNote += "\n---"; // Clear separator
                    }
                    currentNote += $"\nĐã bị từ chối vì: \n{rejectReason}";
                }
                bloodRequest.Note = currentNote;
                bloodRequest.UpdatedAt = DateTime.UtcNow;

                await _bloodRequestRepository.UpdateAsync(bloodRequest);
                await _bloodRequestRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Improved logging for better debugging
                Console.WriteLine($"Error rejecting blood request ID {requestId}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> SoftDeleteBloodRequestAsync(int requestId)
        {
            try
            {
                return await _bloodRequestRepository.SoftDeleteBloodRequestAsync(requestId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error soft deleting blood request", ex);
                throw;
            }
        }

        public async Task<bool> UpdateBloodRequestStatusAsync(int requestId, int statusId)
        {
            try
            {
                return await _bloodRequestRepository.UpdateBloodRequestStatusAsync(requestId, statusId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating blood request status", ex);
                throw;
            }
        }

        // Auto-assigns blood units to a blood request based on blood type ID prioritization and volume requirements
        public async Task<bool> AutoAssignBloodUnitsToRequestAsync(int requestId)
        {
            try
            {
                // Get the blood request
                var bloodRequest = await _bloodRequestRepository.GetByIdAsync(requestId);
                if (bloodRequest == null || bloodRequest.RequestStatusId != 2) // Only process approved requests
                {
                    return false;
                }

                // Get compatible blood type IDs based on the recipient blood type ID
                var compatibleBloodTypeIds = await _bloodCompatibilityService.GetAllCompatibleDonorBloodTypeIdsAsync(bloodRequest.BloodTypeId);
                if (!compatibleBloodTypeIds.Any())
                {
                    return false;
                }

                // Get prioritized blood type IDs based on recipient blood type ID
                List<int> prioritizedBloodTypeIds = GetPrioritizedBloodTypeIds(bloodRequest.BloodTypeId);

                // Filter to ensure they're compatible and in the right order
                prioritizedBloodTypeIds = prioritizedBloodTypeIds
                    .Where(id => compatibleBloodTypeIds.Contains(id))
                    .ToList();

                // Add any compatible types not in our priority list at the end
                foreach (var typeId in compatibleBloodTypeIds)
                {
                    if (!prioritizedBloodTypeIds.Contains(typeId))
                    {
                        prioritizedBloodTypeIds.Add(typeId);
                    }
                }

                // Get all available blood units with compatible blood types and matching component
                var availableUnits = new List<BloodUnit>();
                foreach (var typeId in prioritizedBloodTypeIds)
                {
                    var units = await _bloodUnitRepository.GetUnitsByBloodTypeIdAsync(typeId);
                    var matchingUnits = units.Where(u =>
                        u.ComponentId == bloodRequest.BloodComponentId &&
                        u.BloodUnitStatusId == 1 && // Available status
                        u.RequestId == null &&
                        u.ExpiryDateTime > DateTime.UtcNow)
                        .ToList();

                    availableUnits.AddRange(matchingUnits);
                }

                if (!availableUnits.Any())
                {
                    return false;
                }

                // Group units by blood type ID and volume for prioritization
                var unitsByTypeAndVolume = new Dictionary<int, Dictionary<decimal, List<BloodUnit>>>();
                foreach (var unit in availableUnits.Where(u => u.Volume.HasValue))
                {
                    if (!unitsByTypeAndVolume.ContainsKey(unit.BloodTypeId))
                    {
                        unitsByTypeAndVolume[unit.BloodTypeId] = new Dictionary<decimal, List<BloodUnit>>();
                    }

                    var volumeValue = unit.Volume.Value;
                    if (!unitsByTypeAndVolume[unit.BloodTypeId].ContainsKey(volumeValue))
                    {
                        unitsByTypeAndVolume[unit.BloodTypeId][volumeValue] = new List<BloodUnit>();
                    }

                    unitsByTypeAndVolume[unit.BloodTypeId][volumeValue].Add(unit);
                }

                // Sort each volume group by expiry date
                foreach (var typeId in unitsByTypeAndVolume.Keys)
                {
                    foreach (var volumeKey in unitsByTypeAndVolume[typeId].Keys)
                    {
                        unitsByTypeAndVolume[typeId][volumeKey] = unitsByTypeAndVolume[typeId][volumeKey]
                            .OrderBy(u => u.ExpiryDateTime)
                            .ToList();
                    }
                }

                decimal remainingVolume = bloodRequest.Volume;
                var assignedUnits = new List<BloodUnit>();

                // Try each blood type in priority order
                foreach (var bloodTypeId in prioritizedBloodTypeIds)
                {
                    if (remainingVolume <= 0)
                        break;

                    // Skip if no units available for this blood type
                    if (!unitsByTypeAndVolume.ContainsKey(bloodTypeId))
                        continue;

                    var volumeUnitsDict = unitsByTypeAndVolume[bloodTypeId];

                    // Apply volume-based prioritization logic
                    while (remainingVolume > 0)
                    {
                        BloodUnit selectedUnit = null;

                        // Volume prioritization logic
                        if (remainingVolume > 450)
                        {
                            // For large volume requests, prioritize 450ml units
                            selectedUnit = GetNextUnitByVolume(volumeUnitsDict, 450);
                        }
                        else if (remainingVolume > 350)
                        {
                            // For medium-large volume requests (350-450ml)
                            selectedUnit = GetNextUnitByVolume(volumeUnitsDict, 450);
                        }
                        else if (remainingVolume > 250)
                        {
                            // For medium volume requests (250-350ml)
                            selectedUnit = GetNextUnitByVolume(volumeUnitsDict, 350);
                        }
                        else
                        {
                            // For small volume requests (<= 250ml)
                            selectedUnit = GetNextUnitByVolume(volumeUnitsDict, 250);
                        }

                        // If no unit with preferred volume is available, try alternatives
                        if (selectedUnit == null)
                        {
                            // Try to find any available unit in order of preference
                            var availableVolumes = volumeUnitsDict.Keys.OrderByDescending(v => v);

                            foreach (var volume in availableVolumes)
                            {
                                selectedUnit = GetNextUnitByVolume(volumeUnitsDict, volume);
                                if (selectedUnit != null) break;
                            }
                        }

                        // If no units available for this blood type, move to next blood type
                        if (selectedUnit == null)
                        {
                            break;
                        }

                        // Assign the unit to the request
                        selectedUnit.RequestId = requestId;
                        selectedUnit.BloodUnitStatusId = 2; // Assuming 2 = "Assigned" status
                        selectedUnit.UpdatedAt = DateTime.UtcNow;

                        assignedUnits.Add(selectedUnit);
                        remainingVolume -= selectedUnit.Volume ?? 0;

                        // Update the unit in repository
                        await _bloodUnitRepository.UpdateAsync(selectedUnit);
                    }

                    // If volume satisfied, break the loop
                    if (remainingVolume <= 0)
                        break;
                }

                // Save all changes
                await _bloodUnitRepository.SaveChangesAsync();

                // Update the request status if units were assigned
                if (assignedUnits.Any())
                {
                    // Calculate total assigned volume
                    decimal assignedVolume = assignedUnits.Sum(u => u.Volume ?? 0);

                    // Update request
                    bloodRequest.Volume -= assignedVolume;
                    bloodRequest.UpdatedAt = DateTime.UtcNow;

                    // If the request is fully satisfied, update its status
                    if (bloodRequest.Volume <= 0)
                    {
                        bloodRequest.RequestStatusId = 4; // Assuming 4 = "Fulfilled" status
                        bloodRequest.Volume = 0; // Set to exactly 0 if fully satisfied
                    }

                    await _bloodRequestRepository.UpdateAsync(bloodRequest);
                    await _bloodRequestRepository.SaveChangesAsync();
                }

                return assignedUnits.Any();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error auto-assigning blood units: {ex.Message}");
                throw;
            }
        }
        private List<int> GetPrioritizedBloodTypeIds(int recipientBloodTypeId)
        {
            switch (recipientBloodTypeId)
            {
                case 8: // O-
                    return new List<int> { 8 };
                case 7: // O+
                    return new List<int> { 7, 8 };
                case 2: // A-
                    return new List<int> { 2, 8 };
                case 1: // A+
                    return new List<int> { 1, 7, 2, 8 };
                case 4: // B-
                    return new List<int> { 4, 8 };
                case 3: // B+
                    return new List<int> { 3, 7, 4, 8 };
                case 6: // AB-
                    return new List<int> { 6, 2, 4, 8 };
                case 5: // AB+
                    return new List<int> { 5, 1, 3, 7, 6, 2, 4, 8 };
                default:
                    return new List<int> { 8 }; // Default to universal donor (O-) if ID unknown
            }
        }

        private BloodUnit GetNextUnitByVolume(Dictionary<decimal, List<BloodUnit>> unitsByVolume, decimal volume)
        {
            if (unitsByVolume.ContainsKey(volume) && unitsByVolume[volume].Any())
            {
                var unit = unitsByVolume[volume].First();
                unitsByVolume[volume].Remove(unit);
                return unit;
            }
            return null;
        }

        public async Task<bool> AutoAssignBloodUnitsToAllPendingRequestsAsync()
        {
            try
            {
                // Get all approved requests that haven't been fulfilled yet (status ID 2 = approved)
                var pendingRequests = await _bloodRequestRepository.GetBloodRequestsByStatusIdAsync(2);

                bool anyAssigned = false;

                // Process requests in order of urgency (if available) and then by request date
                var orderedRequests = pendingRequests
                    .OrderByDescending(r => r.UrgencyId ?? 0)
                    .ThenBy(r => r.RequestDateTime);

                foreach (var request in orderedRequests)
                {
                    var assigned = await AutoAssignBloodUnitsToRequestAsync(request.RequestId);
                    if (assigned)
                    {
                        anyAssigned = true;
                    }
                }

                return anyAssigned;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error auto-assigning blood units to all pending requests: {ex.Message}");
                throw;
            }
        }
    }
}

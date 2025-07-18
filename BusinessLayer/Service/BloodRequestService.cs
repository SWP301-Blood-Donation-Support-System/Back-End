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
                entity.RemainingVolume = bloodRequest.Volume;
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

        public async Task<IEnumerable<BloodRequestResponseDTO>> GetBloodRequestsByBloodTypeIdAsync(int bloodTypeId)
        {
            try
            {
                var requests = await _bloodRequestRepository.GetBloodRequestsByBloodTypeIdAsync(bloodTypeId);
                return _mapper.Map<IEnumerable<BloodRequestResponseDTO>>(requests);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by blood type ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequestResponseDTO>> GetBloodRequestsByComponentIdAsync(int componentId)
        {
            try
            {
                var requests = await _bloodRequestRepository.GetBloodRequestsByComponentIdAsync(componentId);
                return _mapper.Map<IEnumerable<BloodRequestResponseDTO>>(requests);
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
                return await _bloodRequestRepository.GetBloodRequestWithDetailsAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood request by ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequestResponseDTO>> GetBloodRequestsByStaffIdAsync(int staffId)
        {
            try
            {
                var requests = await _bloodRequestRepository.GetBloodRequestsByStaffIdAsync(staffId);
                return _mapper.Map<IEnumerable<BloodRequestResponseDTO>>(requests);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by staff ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequestResponseDTO>> GetBloodRequestsByStatusIdAsync(int statusId)
        {
            try
            {
                var requests = await _bloodRequestRepository.GetBloodRequestsByStatusIdAsync(statusId);
                return _mapper.Map<IEnumerable<BloodRequestResponseDTO>>(requests);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving blood requests by status ID", ex);
                throw;
            }
        }

        public async Task<IEnumerable<BloodRequestResponseDTO>> GetBloodRequestsByUrgencyIdAsync(int urgencyId)
        {
            try
            {
                var requests = await _bloodRequestRepository.GetBloodRequestsByUrgencyIdAsync(urgencyId);
                return _mapper.Map<IEnumerable<BloodRequestResponseDTO>>(requests);
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
                bloodRequest.RequestStatusId = 4;

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

        /// <summary>
        /// Finds blood units that can be assigned to a blood request based on blood type ID prioritization and volume requirements
        /// </summary>
        /// <returns>A list of blood units that could be assigned to the request</returns>
        public async Task<IEnumerable<BloodUnitDTO>> AutoAssignBloodUnitsToRequestAsync(int requestId)
        {
            try
            {
                // Get the blood request
                var bloodRequest = await _bloodRequestRepository.GetByIdAsync(requestId);
                if (bloodRequest == null || bloodRequest.RequestStatusId != 2) // Only process approved requests
                {
                    return new List<BloodUnitDTO>();
                }

                // Get compatible blood type IDs based on the recipient blood type ID
                var compatibleBloodTypeIds = await _bloodCompatibilityService.GetAllCompatibleDonorBloodTypeIdsAsync(bloodRequest.BloodTypeId);
                if (!compatibleBloodTypeIds.Any())
                {
                    return new List<BloodUnitDTO>();
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
                    throw new InvalidOperationException(
                        $"No suitable blood units found for request ID {requestId}. " +
                        $"Requested: {bloodRequest.Volume}ml."
                    );
                }

                // Group units by blood type ID and volume for prioritization
                var unitsByTypeAndVolume = new Dictionary<int, Dictionary<decimal, List<BloodUnit>>>();
                foreach (var unit in availableUnits)
                {
                    if (!unitsByTypeAndVolume.ContainsKey(unit.BloodTypeId))
                    {
                        unitsByTypeAndVolume[unit.BloodTypeId] = new Dictionary<decimal, List<BloodUnit>>();
                    }

                    var volumeValue = unit.Volume;
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

                decimal remainingVolume = (decimal)bloodRequest.RemainingVolume;
                var suggestedUnits = new List<BloodUnitDTO>();

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

                        // Add to suggested units instead of assigning
                        suggestedUnits.Add(_mapper.Map<BloodUnitDTO>(selectedUnit));
                        remainingVolume -= selectedUnit.Volume;
                    }

                    // If volume satisfied, break the loop
                    if (remainingVolume <= 0)
                        break;
                }

                if (suggestedUnits.Any())
                {
                    decimal fulfilledVolume = bloodRequest.Volume - remainingVolume;

                    // Log a warning instead of throwing an exception
                    if (remainingVolume > 0)
                    {
                        Console.WriteLine($"WARNING: Could not fulfill the entire blood request for ID {requestId}. " +
                            $"Requested: {bloodRequest.Volume}ml, Fulfilled: {fulfilledVolume}ml, Remaining: {remainingVolume}ml still needed.");
                    }

                    return suggestedUnits;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"No suitable blood units found for request ID {requestId}. " +
                        $"Requested: {bloodRequest.RemainingVolume}ml."
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding blood units for assignment: {ex.Message}");
                throw;
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
                    if (assigned.Any())
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
        public async Task RefreshRemainingVolume(int requestId)
        {
            try
            {
                var bloodRequest = await _bloodRequestRepository.GetByIdAsync(requestId);
                if (bloodRequest == null)
                {
                    throw new KeyNotFoundException("Blood request not found");
                }
                // Get all assigned blood units for this request
                var assignedUnits = await _bloodUnitRepository.GetUnitsByRequestIdAsync(requestId);
                // Calculate the total volume of assigned units
                decimal totalAssignedVolume = assignedUnits.Sum(u => u.Volume);
                // Update the remaining volume in the request
                bloodRequest.RemainingVolume = bloodRequest.Volume - totalAssignedVolume;
                if(bloodRequest.RemainingVolume <= 0)
                {
                    bloodRequest.RemainingVolume = 0;
                    bloodRequest.RequestStatusId = 3; // Set to Complete
                }
                // If there are still volumes remaining and request status is complete
                // set the status to Approved (2)
                if (bloodRequest.RemainingVolume>0 && bloodRequest.RequestStatusId == 3)
                {
                    bloodRequest.RequestStatusId = 2; // Set to Approved if there are still volumes remaining
                }
                bloodRequest.UpdatedAt = DateTime.UtcNow;
                await _bloodRequestRepository.UpdateAsync(bloodRequest);
                await _bloodRequestRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing remaining volume for request ID {requestId}: {ex.Message}");
                throw;
            }
        }
    }
}

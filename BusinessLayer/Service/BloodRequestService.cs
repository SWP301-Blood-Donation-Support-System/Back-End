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
        private readonly IMapper _mapper;
        public BloodRequestService(IBloodRequestRepository bloodRequestRepository, IMapper mapper)
        {
            _bloodRequestRepository = bloodRequestRepository;
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
            catch(Exception ex)
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
            catch(Exception ex)
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
            catch(Exception ex)
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
    }
}

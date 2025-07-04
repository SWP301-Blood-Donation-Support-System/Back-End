using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IBloodRequestService
    {
        Task AddBloodRequestAsync(BloodRequestDTO bloodRequest);
        Task <bool> UpdateBloodRequestStatusAsync(int requestId, int statusId);
        Task<IEnumerable<BloodRequest>> GetAllBloodRequestsAsync();
        Task<BloodRequest> GetBloodRequestsByIdAsync(int id);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByComponentIdAsync(int componentId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByStatusIdAsync(int statusId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByUrgencyIdAsync(int urgencyId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByStaffIdAsync(int staffId);   
        Task<bool> ApproveBloodRequestAsync(int requestId, int approvedByUserId);
        Task<bool> RejectBloodRequestAsync(int requestId, int rejectedByUserId, string? rejectReason);
        Task<bool> SoftDeleteBloodRequestAsync(int requestId);
        Task<IEnumerable<BloodUnitDTO>> AutoAssignBloodUnitsToRequestAsync(int requestId);
    }
}

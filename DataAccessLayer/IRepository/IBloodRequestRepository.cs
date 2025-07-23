using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IBloodRequestRepository:IGenericRepository<BloodRequest>
    {
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByComponentIdAsync(int componentId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByStatusIdAsync(int statusId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByUrgencyIdAsync(int urgencyId);
        Task<IEnumerable<BloodRequest>> GetBloodRequestsByStaffIdAsync(int staffId);
        Task<bool> UpdateBloodRequestStatusAsync(int requestId, int statusId);
        Task<bool> SoftDeleteBloodRequestAsync(int requestId);
        Task<BloodRequest> GetBloodRequestWithDetailsAsync(int id);
    }
}

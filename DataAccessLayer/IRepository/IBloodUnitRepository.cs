using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IBloodUnitRepository : IGenericRepository<BloodUnit>
    {
        IQueryable<BloodUnit> GetAllAsQueryable();
        Task<IEnumerable<BloodUnit>> GetUnitsByBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<BloodUnit>> GetUnitsByStatusAsync(int bloodUnitStatusId);
        Task<IEnumerable<BloodUnit>> GetUnitsByBloodComponentIdAsync(int bloodComponentId);
        Task<IEnumerable<BloodUnit>> GetUnitsByRecordIdAsync(int recordId);
        Task<IEnumerable<BloodUnit>> GetUnitsByRequestIdAsync(int requestId);
        Task<bool> UpdateUnitStatusAsync(int unitId, int bloodUnitStatusId);
    }
}

using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IBloodUnitService
    {
        Task<IEnumerable<BloodUnit>> GetAllBloodUnitsAsync();
        Task<BloodUnit> GetBloodUnitByIdAsync(int id);
        Task AddBloodUnitAsync(BloodUnitDTO bloodUnit);
        Task<bool> UpdateBloodUnitAsync(BloodUnit bloodUnit);
        Task<bool> DeleteBloodUnitAsync(int id);
        Task<IEnumerable<BloodUnit>> GetBloodUnitsByBloodTypeAsync(int bloodTypeId);
        Task<IEnumerable<BloodUnit>> GetBloodUnitsByBloodComponentAsync(int bloodComponentId);
        Task<IEnumerable<BloodUnit>> GetBloodUnitsByStatusAsync(int statusId);
        Task<bool> UpdateBloodUnitStatusAsync(int unitId, int bloodUnitStatusId);
    }
}

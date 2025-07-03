using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IBloodCompatibilityRepository: IGenericRepository<BloodCompatibility>
    {
        Task<IEnumerable<BloodCompatibility>> GetCompatibilityByDonorBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<BloodCompatibility>> GetCompatibilityByRecipientBloodTypeIdAsync(int bloodTypeId);
        Task<bool> IsCompatibleAsync(int donorBloodTypeId, int recipientBloodTypeId);   

        Task<IEnumerable<int>> GetAllCompatibleDonorBloodTypeIdsAsync(int recipientBloodTypeId);    
    }
}

using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IBloodCompatibilityService
    {
        Task<IEnumerable<int>> GetAllCompatibleDonorBloodTypeIdsAsync(int recipientBloodTypeId);
        Task<bool> IsCompatibleAsync(int donorBloodTypeId, int recipientBloodTypeId);
        Task<IEnumerable<BloodCompatibility>> GetCompatibilityByDonorBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<BloodCompatibility>> GetCompatibilityByRecipientBloodTypeIdAsync(int bloodTypeId);
    }
}

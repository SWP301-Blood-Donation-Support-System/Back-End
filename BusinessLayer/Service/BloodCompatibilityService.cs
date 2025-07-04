using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class BloodCompatibilityService : IBloodCompatibilityService
    {
        private readonly IBloodCompatibilityRepository _bloodCompatibilityRepository;
        
        public BloodCompatibilityService(IBloodCompatibilityRepository bloodCompatibilityRepository)
        {
            _bloodCompatibilityRepository = bloodCompatibilityRepository;
        }
        public async Task<IEnumerable<int>> GetAllCompatibleDonorBloodTypeIdsAsync(int recipientBloodTypeId)
        {
            return await _bloodCompatibilityRepository.GetAllCompatibleDonorBloodTypeIdsAsync(recipientBloodTypeId);
        }

        public async Task<IEnumerable<BloodCompatibility>> GetCompatibilityByDonorBloodTypeIdAsync(int bloodTypeId)
        {
            return await _bloodCompatibilityRepository.GetCompatibilityByDonorBloodTypeIdAsync(bloodTypeId);
        }

        public async Task<IEnumerable<BloodCompatibility>> GetCompatibilityByRecipientBloodTypeIdAsync(int bloodTypeId)
        {
            return await _bloodCompatibilityRepository.GetCompatibilityByRecipientBloodTypeIdAsync(bloodTypeId);
        }

        public async Task<bool> IsCompatibleAsync(int donorBloodTypeId, int recipientBloodTypeId)
        {
            return await _bloodCompatibilityRepository.IsCompatibleAsync(donorBloodTypeId, recipientBloodTypeId);
        }
    }
}

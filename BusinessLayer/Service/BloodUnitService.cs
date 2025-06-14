using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class BloodUnitService: IBloodUnitService
    {
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IMapper _mapper;
        public BloodUnitService(IBloodUnitRepository bloodUnitRepository, IMapper mapper)
        {
            _bloodUnitRepository = bloodUnitRepository ?? throw new ArgumentNullException(nameof(bloodUnitRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddBloodUnitAsync(BloodUnitDTO bloodUnit)
        {
            try
            {
                var entity = _mapper.Map<BloodUnit>(bloodUnit);
                entity.BloodUnitStatusId = 1;
                // 1 is the default status for new blood units
                await _bloodUnitRepository.AddAsync(entity);
                await _bloodUnitRepository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding blood unit: {ex.Message}");
                throw;
            }
        }

        public Task<bool> DeleteBloodUnitAsync(int id)
        {
            try
            {
                var bloodUnit = _bloodUnitRepository.GetByIdAsync(id);
                if (bloodUnit == null)
                {
                    throw new KeyNotFoundException("Blood unit not found.");
                }
                _bloodUnitRepository.DeleteAsync(id);
                return _bloodUnitRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the blood unit.", ex);
            }
        }

        public async Task<IEnumerable<BloodUnit>> GetAllBloodUnitsAsync()
        {
            return await _bloodUnitRepository.GetAllAsync();
        }

        public async Task<BloodUnit> GetBloodUnitByIdAsync(int id)
        {
            return await _bloodUnitRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Blood unit not found.");
        }

        public async Task<IEnumerable<BloodUnit>> GetBloodUnitsByBloodComponentAsync(int bloodComponentId)
        {
            return await _bloodUnitRepository.GetUnitsByBloodComponentIdAsync(bloodComponentId);
        }

        public async Task<IEnumerable<BloodUnit>> GetBloodUnitsByBloodTypeAsync(int bloodTypeId)
        {
            return await _bloodUnitRepository.GetUnitsByBloodTypeIdAsync(bloodTypeId);
        }

        public async Task<IEnumerable<BloodUnit>> GetBloodUnitsByStatusAsync(int statusId)
        {
            return await _bloodUnitRepository.GetUnitsByStatusAsync(statusId);
        }

        public async Task<bool> UpdateBloodUnitAsync(BloodUnit bloodUnit)
        {
            bloodUnit.UpdatedAt = DateTime.UtcNow;

            await _bloodUnitRepository.UpdateAsync(bloodUnit);
            await _bloodUnitRepository.SaveChangesAsync();
            return true;
        }

        public Task<bool> UpdateBloodUnitStatusAsync(int unitId, int bloodUnitStatusId)
        {
            return _bloodUnitRepository.UpdateUnitStatusAsync(unitId, bloodUnitStatusId);
        }

      
    }
}

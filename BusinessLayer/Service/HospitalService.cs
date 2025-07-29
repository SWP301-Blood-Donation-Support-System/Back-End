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
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IMapper _mapper;

        public HospitalService(IHospitalRepository hospitalRepository, IMapper mapper)
        {
            _hospitalRepository = hospitalRepository ?? throw new ArgumentNullException(nameof(hospitalRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

      
        public async Task AddHospitalAsync(HospitalDTO hospital)
        {
            if (hospital == null)
            {
                throw new ArgumentNullException(nameof(hospital), "Hospital cannot be null");
            }
            var entity = _mapper.Map<Hospital>(hospital);
            entity.CreatedAt = DateTime.UtcNow.AddHours(7);;
            entity.IsDeleted = false;
            await _hospitalRepository.AddAsync(entity);
            await _hospitalRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            return await _hospitalRepository.GetAllAsync();
        }

        public async Task<Hospital> GetHospitalByIdAsync(int hospitalId)
        {
            return await _hospitalRepository.GetByIdAsync(hospitalId);
        }

        public async Task<bool> SoftDeleteHospitalAsync(int hospitalId)
        {
            if (hospitalId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hospitalId), "Hospital ID must be greater than zero");
            }
            return await _hospitalRepository.SoftDeleteHospitalAsync(hospitalId);
        }

        public async Task<bool> UpdateHospitalAsync(int hospitalId, HospitalDTO hospital)
        {
            if (hospital == null)
            {
                throw new ArgumentNullException(nameof(hospital), "Hospital cannot be null");
            }
            if (hospitalId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hospitalId), "Hospital ID must be greater than zero");
            }
            var existingHospital = await _hospitalRepository.GetByIdAsync(hospitalId);
            if (existingHospital == null)
            {
                throw new KeyNotFoundException($"Hospital with ID {hospitalId} not found");
            }
            existingHospital.HospitalName=hospital.HospitalName;
            existingHospital.HospitalAddress = hospital.HospitalAddress;
            existingHospital.UpdatedAt = DateTime.UtcNow.AddHours(7);;
            await _hospitalRepository.UpdateAsync(existingHospital);
            await _hospitalRepository.SaveChangesAsync();
            return true;
        }
    }
}

using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IHospitalService
    {
        Task AddHospitalAsync(HospitalDTO hospital);
        Task<IEnumerable<Hospital>> GetAllHospitalsAsync();
        Task<Hospital> GetHospitalByIdAsync(int hospitalId);
        Task<bool> SoftDeleteHospitalAsync(int hospitalId);
        Task<bool> UpdateHospitalAsync(int hospitalId, HospitalDTO hospital); 
    }
}

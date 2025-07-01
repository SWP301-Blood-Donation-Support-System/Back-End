using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IHospitalRepository:IGenericRepository<Hospital>
    {
        Task<bool> SoftDeleteHospitalAsync(int hospitalId);
    }
}

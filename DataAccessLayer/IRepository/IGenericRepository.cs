using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<T> GetByIdAsync(int id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> AddAsync(T entity);
        public Task<T> UpdateAsync(T entity);
        public Task<bool> DeleteAsync(int id);
        public Task<bool> SaveChangesAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<T> GetByIdAsync<T>(int id) where T : class;
        public Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        public Task<T> AddAsync<T>(T entity) where T : class;
        public Task<T> UpdateAsync<T>(T entity) where T : class;
        public Task<bool> DeleteAsync<T>(int id) where T : class;
        public Task<bool> SaveChangesAsync();


    }
}

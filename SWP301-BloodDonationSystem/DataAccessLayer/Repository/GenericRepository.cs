using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public GenericRepository() { }
        private readonly BloodDonationDbContext _context;
        public Task<T> AddAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            return Task.FromResult(entity);

        }

        public Task<bool> DeleteAsync<T>(int id) where T : class
        {
            var entity = _context.Set<T>().Find(id);
            if (entity == null)
            {
                return Task.FromResult(false);
            }
            _context.Set<T>().Remove(entity);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
                     return Task.FromResult(_context.Set<T>().AsEnumerable());
        }

        public Task<T> GetByIdAsync<T>(int id) where T : class
        {
         
            var entity = _context.Set<T>().Find(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found");
            }
            return Task.FromResult(entity);
        }

        public Task<bool> SaveChangesAsync()
        {
         
            try
            {
                return Task.FromResult(_context.SaveChanges() > 0);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
        }

        public Task<T> UpdateAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            return Task.FromResult(entity);
        }
    }
}

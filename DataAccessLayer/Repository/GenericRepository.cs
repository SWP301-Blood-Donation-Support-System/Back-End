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
        private readonly BloodDonationDbContext _context;

        public GenericRepository(BloodDonationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<T> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _context.Set<T>().Add(entity);
                return Task.FromResult(entity);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Task.FromResult<T>(null);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity == null)
            {
                return Task.FromResult(false);
            }
            _context.Set<T>().Remove(entity);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_context.Set<T>().AsEnumerable());
        }

        public Task<T> GetByIdAsync(int id)
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
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                // Log the exception (not implemented here)
                throw new Exception($"Database error: {errorMessage}", ex);
            }
        }

        public Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Update(entity);
            return Task.FromResult(entity);
        }

    }
}

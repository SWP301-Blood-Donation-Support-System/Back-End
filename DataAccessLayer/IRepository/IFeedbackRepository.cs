using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entity;

namespace DataAccessLayer.IRepository
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {
        Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();
        Task<Feedback> GetFeedbackByIdAsync(int feedId);
        Task<IEnumerable<Feedback>> GetFeedbackByRegistrationIdAsync(int registrationId);

        Task<bool> SoftDeleteFeedbackAsync(int feedId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        private readonly BloodDonationDbContext _context;
        public FeedbackRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Feedback> AddAsync(Feedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException(nameof(feedback));
            await _context.Feedbacks.AddAsync(feedback);
            return feedback;
        }
        public async Task<Feedback> GetFeedbackByIdAsync(int feedId)
        {
            return await _context.Feedbacks.FindAsync(feedId);
        }
        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks.ToListAsync();
        }
        public async Task<IEnumerable<Feedback>> GetFeedbackByRegistrationIdAsync(int registrationId)
        {
            return await _context.Feedbacks
                .Where(f => f.RegistrationId == registrationId)
                .ToListAsync();
        }
        public async Task<bool> SoftDeleteFeedbackAsync(int feedId)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedId);
            if (feedback == null)
            {
                return false; // Feedback not found
            }
            feedback.IsDeleted = true; // Soft delete
            _context.Feedbacks.Update(feedback);
            return await _context.SaveChangesAsync() > 0; // Save changes
        }
    }
}

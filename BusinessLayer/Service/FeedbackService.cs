using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;

namespace BusinessLayer.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
        }
        public async Task<bool> AddAsync(Feedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException(nameof(feedback));
            await _feedbackRepository.AddAsync(feedback);
            return await _feedbackRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            return await _feedbackRepository.GetAllFeedbacksAsync();
        }
        public async Task<Feedback> GetFeedbackByIdAsync(int feedId)
        {
            if (feedId <= 0)
                throw new ArgumentOutOfRangeException(nameof(feedId), "ID must be greater than zero");
            return await _feedbackRepository.GetFeedbackByIdAsync(feedId);
        }
        public async Task<Feedback?> GetFeedbackByDonorIdAsync(int donorId)
        {
            if (donorId <= 0)
                throw new ArgumentOutOfRangeException(nameof(donorId), "ID must be greater than zero");

            var feedbacks = await _feedbackRepository.GetFeedbackByDonorIdAsync(donorId);
            return feedbacks.FirstOrDefault(); // Return the first feedback or null if none exist
        }

        public async Task<bool> SoftDeleteFeedbackAsync(int feedId)
        {
            if (feedId <= 0)
                throw new ArgumentOutOfRangeException(nameof(feedId), "ID must be greater than zero");
            return await _feedbackRepository.SoftDeleteFeedbackAsync(feedId);
        }
    }
}

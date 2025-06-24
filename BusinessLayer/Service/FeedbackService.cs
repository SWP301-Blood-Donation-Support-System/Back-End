using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;

namespace BusinessLayer.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper Mapper;
        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper)
        {

            _feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
            Mapper = mapper;
        }
        public async Task<bool> AddAsync(FeedbackDTO feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException(nameof(feedback));
            var entity = Mapper.Map<Feedback>(feedback);
            await _feedbackRepository.AddAsync(entity);
            return await _feedbackRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            var entity = await _feedbackRepository.GetAllFeedbacksAsync();
            return Mapper.Map<IEnumerable<FeedbackDTO>>(entity);
        }
        public async Task<Feedback> GetFeedbackByIdAsync(int feedId)
        {
            if (feedId <= 0)
                throw new ArgumentOutOfRangeException(nameof(feedId), "ID must be greater than zero");
            return await _feedbackRepository.GetFeedbackByIdAsync(feedId);
        }
        public async Task<Feedback?> GetFeedbackByRegistrationIdAsync(int registrationId)
        {
            if (registrationId <= 0)
                throw new ArgumentOutOfRangeException(nameof(registrationId), "ID must be greater than zero");

            var feedbacks = await _feedbackRepository.GetFeedbackByRegistrationIdAsync(registrationId);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BusinessLayer.IService
{
    public interface IFeedbackService
    {
        /// <summary>
        /// Adds a new feedback.
        /// </summary>
        /// <param name="feedback">The feedback to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<bool> AddAsync(FeedbackDTO feedback);
        /// <summary>
        /// Gets all feedbacks.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of feedbacks.</returns>
        Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync();
        /// <summary>
        /// Gets feedback by ID.
        /// </summary>
        /// <param name="feedId">The ID of the feedback.</param>
        /// <returns>A task representing the asynchronous operation, containing the feedback if found.</returns>
        Task<Feedback> GetFeedbackByIdAsync(int feedId);
        /// <summary>
        /// Gets feedback by donor ID.
        /// </summary>
        /// <param name="registrationId"> The ID of the registration.</param>
        /// <returns>A task representing the asynchronous operation, containing the feedback if found.</returns>
        Task<Feedback?> GetFeedbackByRegistrationIdAsync(int registrationId);
        /// <summary>
        /// Soft delete the feedback.
        /// </summary>
        /// <param name="feedId">The ID of the feedback.</param>
        /// <return>Soft delete of the feedback, isDeleted="1"</return>
        Task<bool> SoftDeleteFeedbackAsync(int feedId);
    }
}

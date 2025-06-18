using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface ITimeSlotServices
    {
        Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync();
        Task<TimeSlot> GetTimeSlotByIdAsync(int id);
        Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync();
        Task<bool> AddTimeSlotAsync(TimeSlot timeSlot);
        Task<bool> UpdateTimeSlotAsync(TimeSlot timeSlot);
        Task<bool> DeleteTimeSlotAsync(int timeSlotId);
        Task<bool> SaveChangesAsync();
    }
}

using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class TimeSlotServices : ITimeSlotServices
    {
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;
        public TimeSlotServices(ITimeSlotRepository timeSlotRepository, IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public Task<bool> AddTimeSlotAsync(TimeSlot timeSlot)
        {
            _timeSlotRepository.AddAsync(timeSlot);
            return _timeSlotRepository.SaveChangesAsync();
        }

        public Task<bool> DeleteTimeSlotAsync(int timeSlotId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync()
        {
            return await _timeSlotRepository.GetAllAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync()
        {
            return await _timeSlotRepository.GetAvailableTimeSlotsAsync();
        }

        public Task<bool> SaveChangesAsync()
        {
            return _timeSlotRepository.SaveChangesAsync();
        }

        public Task<bool> UpdateTimeSlotAsync(TimeSlot timeSlot)
        {
            throw new NotImplementedException();
        }

        public Task<TimeSlot> GetTimeSlotByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            return _timeSlotRepository.GetByIdAsync(id);
        }
    }
}

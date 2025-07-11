﻿using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
     public interface ITimeSlotRepository : IGenericRepository<TimeSlot>
     {
        Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync();
    }
}

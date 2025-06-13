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
    public class DonationScheduleService 
    {
        private readonly IDonationScheduleRepository _donationScheduleRepository;

        public DonationScheduleService(IDonationScheduleRepository donationScheduleRepository)
        {
            _donationScheduleRepository = donationScheduleRepository;
        }

        
    }
}

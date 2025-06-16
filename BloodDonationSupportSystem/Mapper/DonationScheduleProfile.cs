using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class DonationScheduleProfile : Profile
    {
        public DonationScheduleProfile()
        {
            // Map from Entity to DTO (only mapping ScheduleId and ScheduleDate)
            CreateMap<DonationSchedule, DonationScheduleDTO>();
           
        }
    }
}
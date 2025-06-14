using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class DonationRecordProfile : Profile
    {
        public DonationRecordProfile()
        {
            CreateMap<DonationRecord, DonationRecordDTO>().ReverseMap();
        }
    }
}

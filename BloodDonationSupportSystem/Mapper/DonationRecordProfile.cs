using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class DonationRecordProfile : Profile
    {
        public DonationRecordProfile()
        {
            CreateMap<DonationValidation, DonationValidationDTO>().ReverseMap();

            CreateMap<DonationRecord, DonationRecordDTO>().ReverseMap();

            CreateMap<DonationRecordUpdateDTO, DonationRecord>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class BloodUnitProfile:Profile
    {
        public BloodUnitProfile()
        {
            CreateMap<BloodUnit,BloodUnitDTO>().ReverseMap();
            CreateMap<BloodUnit, BloodUnitResponseDTO>()
                .ForMember(dest => dest.BloodTypeName, opt => opt.MapFrom(src => src.BloodType.BloodTypeName))
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(src => src.Component.ComponentName))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.BloodUnitStatus.StatusName))
                .ForMember(dest => dest.DonorName, opt => opt.MapFrom(src => src.DonationRecord.Registration.Donor.FullName));
        }
    }
}

using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class DonationRegistrationProfile : Profile
    {
        public DonationRegistrationProfile()
        {
            // Existing mappings (if any)
            CreateMap<DonationRegistration, DonationRegistrationDTO>().ReverseMap();
            
            // Map from entity to response DTO
            CreateMap<DonationRegistration, DonationRegistrationResponseDTO>();
            
            // Map from User to DonorBasicInfoDTO
            CreateMap<User, DonorBasicInfoDTO>();
            
            // Map other related entities to their DTOs
            CreateMap<TimeSlot, TimeSlotDTO>()
                .ForMember(dest => dest.TimeSlotId, opt => opt.MapFrom(src => src.TimeSlotId))
                .ForMember(dest => dest.TimeSlotName, opt => opt.MapFrom(src => src.TimeSlotName))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString()))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString()))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.DonationRegistrations != null ? src.DonationRegistrations.Count : 0))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));
            
            CreateMap<RegistrationStatus, RegistrationStatusDTO>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.RegistrationStatusId))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.RegistrationStatusName));
        }
    }
}

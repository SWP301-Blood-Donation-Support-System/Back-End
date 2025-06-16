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
            
            // Map from DTO to Entity (maintaining other entity properties during updates)
            CreateMap<DonationScheduleDTO, DonationSchedule>()
                .ForMember(dest => dest.RegisteredSlots, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DonationRegistrations, opt => opt.Ignore());
        }
    }
}
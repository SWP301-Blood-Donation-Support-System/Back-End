using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class LookupProfile : Profile
    {
        public LookupProfile()
        {
            // Map all entity types to LookupDTO
            CreateMap<ArticleCategory, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ArticleCategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<ArticleStatus, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ArticleStatusId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StatusName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<BloodComponent, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ComponentId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ComponentName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<BloodRequestStatus, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BloodRequestStatusId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StatusName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<BloodTestResult, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ResultId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ResultName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<BloodType, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BloodTypeId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BloodTypeName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<BloodUnitStatus, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BloodUnitStatusId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StatusName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<DonationAvailability, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AvailabilityId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AvailabilityName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<DonationType, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DonationTypeId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TypeName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Gender, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.GenderId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GenderName));

            CreateMap<NotificationType, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NotificationTypeId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TypeName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Occupation, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OccupationId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OccupationName));

            CreateMap<RegistrationStatus, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RegistrationStatusId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RegistrationStatusName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Role, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


            CreateMap<Urgency, LookupDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UrgencyId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UrgencyName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}
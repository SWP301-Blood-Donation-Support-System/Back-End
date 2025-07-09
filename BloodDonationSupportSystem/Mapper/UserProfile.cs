using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class UserProfile : Profile
    {
       public UserProfile()
        {
            CreateMap<User, RegisterDTO>().ReverseMap();
            CreateMap<User, LoginDTO>().ReverseMap();
            CreateMap<User, StaffRegisterDTO>().ReverseMap();
            CreateMap<User, DonorDTO>().ReverseMap();
            CreateMap<User, HospitalRegisterDTO>().ReverseMap();

        }
    }
}

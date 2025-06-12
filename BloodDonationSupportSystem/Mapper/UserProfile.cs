using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class UserProfile : Profile
    {
       public UserProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, RegisterDTO>().ReverseMap();

        }
    }
}

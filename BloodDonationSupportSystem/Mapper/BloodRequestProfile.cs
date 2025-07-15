using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class BloodRequestProfile:Profile
    {
        public BloodRequestProfile()
        {
            CreateMap<BloodRequest, BloodRequestDTO>().ReverseMap();
            CreateMap<BloodRequest, BloodRequestResponseDTO>().ReverseMap();
        }
    }
}

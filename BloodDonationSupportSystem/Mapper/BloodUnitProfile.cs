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
        }
    }
}

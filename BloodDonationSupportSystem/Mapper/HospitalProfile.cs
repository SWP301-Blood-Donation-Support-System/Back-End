using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class HospitalProfile:Profile
    {
        public HospitalProfile()
        {
            CreateMap<Hospital,HospitalDTO>().ReverseMap();
        }
    }
}

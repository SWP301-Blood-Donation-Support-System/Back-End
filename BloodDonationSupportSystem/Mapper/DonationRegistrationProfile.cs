using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class DonationRegistrationProfile: Profile
    {
        public DonationRegistrationProfile()
        {
            CreateMap<DonationRegistration,DonationRegistrationDTO>().ReverseMap();
        }
    }
}

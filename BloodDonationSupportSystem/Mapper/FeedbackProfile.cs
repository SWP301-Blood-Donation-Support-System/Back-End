using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BloodDonationSupportSystem.Mapper
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            // Define your mappings here if needed
            // CreateMap<SourceType, DestinationType>();
            CreateMap<Feedback, FeedbackDTO>().ReverseMap();
        }
    }

}

using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class LookupService: ILookupService
    {
        private readonly IGenericRepository<Gender> _genderRepository;
        private readonly IGenericRepository<BloodType> _bloodTypeRepository;
        private readonly IGenericRepository<BloodComponent> _bloodComponentRepository;
        private readonly IGenericRepository<BloodUnitStatus> _bloodUnitStatusRepository;
        private readonly IGenericRepository<DonationAvailability> _donationAvailabilityRepository;
        private readonly IGenericRepository<DonationType> _donationTypeRepository;
        private readonly IGenericRepository<NotificationType> _notificationTypeRepository;
        private readonly IGenericRepository<Occupation> _occupationRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<Urgency> _urgencyRepository;
        private readonly IGenericRepository<BloodRequestStatus> _bloodRequestStatusRepository;
        private readonly IGenericRepository<RegistrationStatus> _registrationStatusRepository;
        private readonly IGenericRepository<ArticleCategory> _articleCategoryRepository;
        private readonly IGenericRepository<ArticleStatus> _articleStatusRepository;
        private readonly IGenericRepository<BloodTestResult> _bloodTestResultRepository;
        private readonly IMapper _mapper;

        public LookupService(IGenericRepository<Gender> genderRepository,
            IGenericRepository<BloodType> bloodTypeRepository,
            IGenericRepository<BloodComponent> bloodComponentRepository,
            IGenericRepository<BloodUnitStatus> bloodUnitStatusRepository, 
            IGenericRepository<DonationAvailability> donationAvailabilityRepository,
            IGenericRepository<DonationType> donationTypeRepository, 
            IGenericRepository<NotificationType> notificationTypeRepository, 
            IGenericRepository<Occupation> occupationRepository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<Urgency> urgencyRepository,
            IGenericRepository<BloodRequestStatus> bloodRequestStatusRepository,
            IGenericRepository<RegistrationStatus> registrationStatusRepository,
            IGenericRepository<ArticleCategory> articleCategoryRepository,
            IGenericRepository<ArticleStatus> articleStatusRepository, 
            IGenericRepository<BloodTestResult> bloodTestResultRepository,
            IMapper mapper)
        {
            _genderRepository = genderRepository;
            _bloodTypeRepository = bloodTypeRepository;
            _bloodComponentRepository = bloodComponentRepository;
            _bloodUnitStatusRepository = bloodUnitStatusRepository;
            _donationAvailabilityRepository = donationAvailabilityRepository;
            _donationTypeRepository = donationTypeRepository;
            _notificationTypeRepository = notificationTypeRepository;
            _occupationRepository = occupationRepository;
            _roleRepository = roleRepository;
            _urgencyRepository = urgencyRepository;
            _bloodRequestStatusRepository = bloodRequestStatusRepository;
            _registrationStatusRepository = registrationStatusRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleStatusRepository = articleStatusRepository;
            _bloodTestResultRepository = bloodTestResultRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LookupDTO>> GetArticleCategoriesAsync()
        {
            return await _articleCategoryRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(ac => _mapper.Map<LookupDTO>(ac)));
        }

        public Task<IEnumerable<LookupDTO>> GetArticleStatusesAsync()
        {
            return _articleStatusRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(ars => _mapper.Map<LookupDTO>(ars)));
        }

        public Task<IEnumerable<LookupDTO>> GetBloodComponentsAsync()
        {
            return _bloodComponentRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(bc => _mapper.Map<LookupDTO>(bc)));
        }
        public Task<IEnumerable<LookupDTO>> GetBloodRequestStatusesAsync()
        {
            return _bloodRequestStatusRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(brs => _mapper.Map<LookupDTO>(brs)));
        }

        public Task<IEnumerable<LookupDTO>> GetBloodTestResultsAsync()
        {
            return _bloodTestResultRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(btr => _mapper.Map<LookupDTO>(btr)));
        }

        public Task<IEnumerable<LookupDTO>> GetBloodTypesAsync()
        {
            return _bloodTypeRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(bt => _mapper.Map<LookupDTO>(bt)));
        }

        public Task<IEnumerable<LookupDTO>> GetBloodUnitStatusesAsync()
        {
            return _bloodUnitStatusRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(bus => _mapper.Map<LookupDTO>(bus)));
        }

        public Task<IEnumerable<LookupDTO>> GetDonationAvailabilitiesAsync()
        {
            return _donationAvailabilityRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(da => _mapper.Map<LookupDTO>(da)));
        }

        public Task<IEnumerable<LookupDTO>> GetDonationTypesAsync()
        {
            return _donationTypeRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(dt => _mapper.Map<LookupDTO>(dt)));
        }

        public Task<IEnumerable<LookupDTO>> GetGendersAsync()
        {
            return _genderRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(g => _mapper.Map<LookupDTO>(g)));
        }

        public Task<IEnumerable<LookupDTO>> GetNotificationTypesAsync()
        {
            return _notificationTypeRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(nt => _mapper.Map<LookupDTO>(nt)));
        }

        public Task<IEnumerable<LookupDTO>> GetOccupationsAsync()
        {
            return _occupationRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(o => _mapper.Map<LookupDTO>(o)));
        }

        public Task<IEnumerable<LookupDTO>> GetRegistrationStatusesAsync()
        {
            return _registrationStatusRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(rs => _mapper.Map<LookupDTO>(rs)));
        }

        public Task<IEnumerable<LookupDTO>> GetRolesAsync()
        {
            return _roleRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(r => _mapper.Map<LookupDTO>(r)));
        }

        public Task<IEnumerable<LookupDTO>> GetUrgenciesAsync()
        {
            return _urgencyRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(u => _mapper.Map<LookupDTO>(u)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;

namespace BusinessLayer.Service
{
    public class DonationRecordService : IDonationRecordService
    {
        private readonly IDonationRecordRepository _donationRecordRepository;
        private readonly IMapper _mapper;


        public DonationRecordService(IDonationRecordRepository donationRecordRepository, IMapper mapper)
        {
            _donationRecordRepository = donationRecordRepository ?? throw new ArgumentNullException(nameof(donationRecordRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DonationRecord> GetRecordsByIdAsync(int recordId)
        {
            if (recordId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(recordId), "Record ID must be greater than zero");
            }

            return await _donationRecordRepository.GetByIdAsync(recordId);
        }

        public async Task<IEnumerable<DonationRecord>> GetAllDonationRecordsAsync()
        {
            return await _donationRecordRepository.GetAllAsync();
        }

        public async Task<DonationRecord> AddRecordsAsync(DonationRecordDTO donationRecord)
        {
            if (donationRecord == null)
            {
                throw new ArgumentNullException(nameof(donationRecord), "Record cannot be null");
            }

            // Set creation timestamp

            var donationRecordEntity = _mapper.Map<DonationRecord>(donationRecord);
            var addedRecord = await _donationRecordRepository.AddAsync(donationRecordEntity);
            await _donationRecordRepository.SaveChangesAsync();
            return addedRecord;
        }

        public async Task<bool> UpdateRecordsAsync(DonationRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record), "Record cannot be null");
            }

            // Update timestamp
            record.UpdatedAt = DateTime.UtcNow;

            await _donationRecordRepository.UpdateAsync(record);
            await _donationRecordRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRecordsAsync(int recordId)
        {
            if (recordId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(recordId), "Record ID must be greater than zero");
            }

            var result = await _donationRecordRepository.DeleteAsync(recordId);
            if (result)
            {
                await _donationRecordRepository.SaveChangesAsync();
            }
            return result;
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByRegistrationIdAsync(int registrationId)
        {
            if (registrationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(registrationId), "Registration ID must be greater than zero");
            }

            return await _donationRecordRepository.GetRecordsByRegistrationIdAsync(registrationId);
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeAsync(DateTime donationDateTime)
        {
            return await _donationRecordRepository.GetRecordsByDonationDateTimeAsync(donationDateTime);
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationTypeIdAsync(int donationTypeId)
        {
            if (donationTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(donationTypeId), "Donation Type ID must be greater than zero");
            }
            return await _donationRecordRepository.GetRecordsByDonationTypeIdAsync(donationTypeId);
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByResultAsync(int result)
        {
            if (result < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(result), "Result must be a valid value");
            }
            return await _donationRecordRepository.GetRecordsByResultAsync(result);
        }

        public async Task<bool> SaveChanges()
        {
            return await _donationRecordRepository.SaveChangesAsync();
        }
    }
}

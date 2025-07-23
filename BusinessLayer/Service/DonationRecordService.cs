using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IMapper _mapper;


        public DonationRecordService(IDonationRecordRepository donationRecordRepository, IBloodUnitRepository bloodUnitRepository, IMapper mapper)
        {

            _donationRecordRepository = donationRecordRepository ?? throw new ArgumentNullException(nameof(donationRecordRepository));
            _bloodUnitRepository = bloodUnitRepository ?? throw new ArgumentNullException(nameof(bloodUnitRepository));
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

        public async Task AddRecordsAsync(DonationRecordDTO donationRecord)
        {
            if (donationRecord == null)
            {
                throw new ArgumentNullException(nameof(donationRecord), "Record cannot be null");
            }

            // Set creation timestamp

            var donationRecordEntity = _mapper.Map<DonationRecord>(donationRecord);
            var addedRecord = await _donationRecordRepository.AddAsync(donationRecordEntity);
            await _donationRecordRepository.SaveChangesAsync();
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

        public async Task<bool> UpdateRecordsAsync(int recordId, DonationRecordUpdateDTO updateDto)
        {
            if (updateDto == null)
            {
                throw new ArgumentNullException(nameof(updateDto), "Update data cannot be null");
            }

            if (recordId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(recordId), "Record ID must be greater than zero");
            }

            // Verify record exists
            var existingRecord = await _donationRecordRepository.GetByIdAsync(recordId);
            if (existingRecord == null)
            {
                return false;
            }

            // Map DTO to entity, preserving properties not included in the DTO
            _mapper.Map(updateDto, existingRecord);

            // Update timestamp
            existingRecord.UpdatedAt = DateTime.UtcNow;

            await _donationRecordRepository.UpdateAsync(existingRecord);
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

        // Trong DonationRecordService, triển khai:
        public async Task<bool> ValidateDonationRecordAsync(int recordId, int userId)
        {
            if (recordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(recordId));
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            // Kiểm tra nếu đã validate rồi thì không validate nữa
            var hasValidation = await _donationRecordRepository.HasValidationAsync(recordId, userId);
            if (hasValidation)
                return true; // Đã validate rồi

            var result = await _donationRecordRepository.AddDonationValidationAsync(recordId, userId);
            await _donationRecordRepository.SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<DonationValidation>> GetValidationsForRecordAsync(int recordId)
        {
            if (recordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(recordId));

            return await _donationRecordRepository.GetValidationsForRecordAsync(recordId);
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByValidatorAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            return await _donationRecordRepository.GetRecordsByValidatorAsync(userId);
        }

        public async Task<IEnumerable<DonationRecordDTO>> GetRecordsByUserId(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero");
            }

            return await _donationRecordRepository.GetRecordsByUserIdAsync(userId)
                .ContinueWith(task => task.Result.Select(rc => _mapper.Map<DonationRecordDTO>(rc)));
        }

        public async Task<bool> UpdateRecordsResultAsync(int recordId, int resultId)
        {
            var record = await _donationRecordRepository.GetRecordAndRegistrationAndUserAsync(recordId);
            record.BloodTestResult = resultId;
            record.UpdatedAt = DateTime.UtcNow;

            if (resultId == 2 || resultId == 3) // If result is 2:"approved" or 3:"rejected" (We still save rejected blood)
            {
                if (record.Registration.Donor.BloodTypeId == 1001)
                {
                    throw new InvalidOperationException("Không thể lưu túi máu chưa biết nhóm máu");
                }
                // Get the donor information to determine blood type
                // Create a new blood unit
                var existingBloodUnits = await _bloodUnitRepository.GetUnitsByRecordIdAsync(recordId);
                if (existingBloodUnits.Any())
                {
                    throw new InvalidOperationException("Đã có túi máu được tạo từ hồ sơ này, không thể tạo thêm");
                }
                var bloodUnit = new BloodUnit
                {
                    DonationRecordId = recordId,
                    BloodTypeId = record.Registration.Donor.BloodTypeId ?? 1001, // Default or use test result
                    ComponentId = record.DonationTypeId ?? 1, // Use donation type as component ID
                    Volume = record.VolumeDonated, // Use the volume donated
                    CollectedDateTime = record.DonationDateTime,
                };
                if (resultId == 2)
                {
                    bloodUnit.BloodUnitStatusId = 1;
                    if (record.DonationTypeId.HasValue)
                    {
                        switch (record.DonationTypeId.Value)
                        {
                            case 1: // Whole Blood
                            case 4: // Red Blood Cells
                                bloodUnit.ExpiryDateTime = record.DonationDateTime.AddDays(42);
                                break;
                            case 2: // Plasma
                                bloodUnit.ExpiryDateTime = record.DonationDateTime.AddYears(1);
                                break;
                            case 3: // Platelets
                                bloodUnit.ExpiryDateTime = record.DonationDateTime.AddDays(5);
                                break;
                            default:
                                // Use a default expiration if component type is unknown
                                bloodUnit.ExpiryDateTime = record.DonationDateTime.AddDays(42);
                                break;
                        }
                    }
                }else if (resultId == 3)
                {
                    bloodUnit.BloodUnitStatusId = 4;
                    bloodUnit.ExpiryDateTime = record.DonationDateTime;
                }
                    // Add the blood unit to the donation record
                    record.BloodUnits.Add(bloodUnit);
            }

            await _donationRecordRepository.UpdateAsync(record);
            return await _donationRecordRepository.SaveChangesAsync();
        }
    }
}
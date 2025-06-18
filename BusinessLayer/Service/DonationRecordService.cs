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

        /// <summary>
        /// Khởi tạo service quản lý phiếu hiến máu với repository và mapper.
        /// </summary>
        public DonationRecordService(IDonationRecordRepository donationRecordRepository, IMapper mapper)
        {
            _donationRecordRepository = donationRecordRepository ?? throw new ArgumentNullException(nameof(donationRecordRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Lấy thông tin phiếu hiến máu theo ID.
        /// </summary>
        public async Task<DonationRecord> GetRecordsByIdAsync(int recordId)
        {
            if (recordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(recordId), "ID phải lớn hơn 0");

            return await _donationRecordRepository.GetByIdAsync(recordId);
        }

        /// <summary>
        /// Lấy danh sách tất cả phiếu hiến máu.
        /// </summary>
        public async Task<IEnumerable<DonationRecord>> GetAllDonationRecordsAsync()
        {
            return await _donationRecordRepository.GetAllAsync();
        }

        /// <summary>
        /// Thêm mới một phiếu hiến máu.
        /// </summary>
        public async Task AddRecordsAsync(DonationRecordDTO donationRecord)
        {
            if (donationRecord == null)
                throw new ArgumentNullException(nameof(donationRecord), "Không được để trống thông tin phiếu");

            var donationRecordEntity = _mapper.Map<DonationRecord>(donationRecord);
            await _donationRecordRepository.AddAsync(donationRecordEntity);
            await _donationRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật thông tin phiếu hiến máu.
        /// </summary>
        public async Task<bool> UpdateRecordsAsync(DonationRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record), "Không được để trống thông tin phiếu");

            record.UpdatedAt = DateTime.UtcNow;

            await _donationRecordRepository.UpdateAsync(record);
            await _donationRecordRepository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cập nhật thông tin phiếu hiến máu theo các trường được chỉ định.
        /// </summary>
        public async Task<bool> UpdateRecordByFieldsAsync(DonationRecordUpdateDTO updateDTO)
        {
            if (updateDTO == null)
                throw new ArgumentNullException(nameof(updateDTO), "Thông tin cập nhật không được để trống");
            
            if (updateDTO.RecordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(updateDTO.RecordId), "ID phiếu hiến máu phải lớn hơn 0");
            
            // Lấy bản ghi hiện tại từ cơ sở dữ liệu
            var existingRecord = await _donationRecordRepository.GetByIdAsync(updateDTO.RecordId);
            if (existingRecord == null)
                return false;
            
            // Cập nhật các trường nếu chúng được cung cấp trong DTO
            if (updateDTO.DonationDateTime.HasValue)
                existingRecord.DonationDateTime = updateDTO.DonationDateTime.Value;
            
            if (updateDTO.DonorWeight.HasValue)
                existingRecord.DonorWeight = updateDTO.DonorWeight.Value;
            
            if (updateDTO.DonorTemperature.HasValue)
                existingRecord.DonorTemperature = updateDTO.DonorTemperature.Value;
            
            if (updateDTO.DonorBloodPressure != null)
                existingRecord.DonorBloodPressure = updateDTO.DonorBloodPressure;
            
            if (updateDTO.DonationTypeId.HasValue)
                existingRecord.DonationTypeId = updateDTO.DonationTypeId.Value;
            
            if (updateDTO.VolumeDonated.HasValue)
                existingRecord.VolumeDonated = updateDTO.VolumeDonated.Value;
            
            if (updateDTO.Note != null)
                existingRecord.Note = updateDTO.Note;
            
            if (updateDTO.BloodTestResult.HasValue)
                existingRecord.BloodTestResult = updateDTO.BloodTestResult.Value;
            
            // Cập nhật thời gian cập nhật
            existingRecord.UpdatedAt = DateTime.UtcNow;
            
            // Gọi repository để cập nhật bản ghi
            await _donationRecordRepository.UpdateAsync(existingRecord);
            await _donationRecordRepository.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Xoá phiếu hiến máu theo ID.
        /// </summary>
        public async Task<bool> DeleteRecordsAsync(int recordId)
        {
            if (recordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(recordId), "ID phải lớn hơn 0");

            var result = await _donationRecordRepository.DeleteAsync(recordId);
            if (result)
            {
                await _donationRecordRepository.SaveChangesAsync();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách phiếu theo ID đăng ký hiến máu.
        /// </summary>
        public async Task<IEnumerable<DonationRecord>> GetRecordsByRegistrationIdAsync(int registrationId)
        {
            if (registrationId <= 0)
                throw new ArgumentOutOfRangeException(nameof(registrationId), "ID đăng ký phải lớn hơn 0");

            return await _donationRecordRepository.GetRecordsByRegistrationIdAsync(registrationId);
        }

        /// <summary>
        /// Lấy danh sách phiếu theo ngày giờ hiến máu.
        /// </summary>
        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeAsync(DateTime donationDateTime)
        {
            return await _donationRecordRepository.GetRecordsByDonationDateTimeAsync(donationDateTime);
        }

        /// <summary>
        /// Lấy danh sách phiếu theo loại hiến máu.
        /// </summary>
        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationTypeIdAsync(int donationTypeId)
        {
            if (donationTypeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(donationTypeId), "ID loại hiến máu phải lớn hơn 0");

            return await _donationRecordRepository.GetRecordsByDonationTypeIdAsync(donationTypeId);
        }

        /// <summary>
        /// Lấy danh sách phiếu theo kết quả hiến máu (ví dụ: đạt hay không đạt).
        /// </summary>
        public async Task<IEnumerable<DonationRecord>> GetRecordsByResultAsync(int result)
        {
            if (result < 0)
                throw new ArgumentOutOfRangeException(nameof(result), "Kết quả không hợp lệ");

            return await _donationRecordRepository.GetRecordsByResultAsync(result);
        }

        /// <summary>
        /// Lưu thay đổi vào cơ sở dữ liệu.
        /// </summary>
        public async Task<bool> SaveChanges()
        {
            return await _donationRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Thực hiện xác nhận phiếu hiến máu bởi một nhân viên.
        /// </summary>
        public async Task<bool> ValidateDonationRecordAsync(int recordId, int userId)
        {
            if (recordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(recordId));
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            var hasValidation = await _donationRecordRepository.HasValidationAsync(recordId, userId);
            if (hasValidation)
                return true;

            var result = await _donationRecordRepository.AddDonationValidationAsync(recordId, userId);
            await _donationRecordRepository.SaveChangesAsync();
            return result;
        }

        /// <summary>
        /// Lấy danh sách xác nhận của một phiếu hiến máu.
        /// </summary>
        public async Task<IEnumerable<DonationValidation>> GetValidationsForRecordAsync(int recordId)
        {
            if (recordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(recordId));

            return await _donationRecordRepository.GetValidationsForRecordAsync(recordId);
        }

        /// <summary>
        /// Lấy danh sách phiếu đã được xác nhận bởi một người dùng (nhân viên xác nhận).
        /// </summary>
        public async Task<IEnumerable<DonationRecord>> GetRecordsByValidatorAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            return await _donationRecordRepository.GetRecordsByValidatorAsync(userId);
        }

        /// <summary>
        /// Lấy danh sách phiếu hiến máu của một người dùng (người hiến).
        /// </summary>
        public async Task<IEnumerable<DonationRecordDTO>> GetRecordsByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId), "ID người dùng phải lớn hơn 0");

            return await _donationRecordRepository.GetRecordsByUserIdAsync(userId)
                .ContinueWith(task => task.Result.Select(rc => _mapper.Map<DonationRecordDTO>(rc)));
        }
    }
}


//------------------------------------------------------------------------------
//  BloodUnitService.cs
//  Triển khai nghiệp vụ cho thực thể BloodUnit
//------------------------------------------------------------------------------
using AutoMapper;                       // Thư viện ánh xạ DTO ↔ Entity
using BusinessLayer.IService;           // Interface dịch vụ
using DataAccessLayer.DTO;              // Các lớp DTO
using DataAccessLayer.Entity;           // Các entity tương ứng bảng DB
using DataAccessLayer.IRepository;      // Interface repo
using System;                           // Kiểu dữ liệu cơ bản
using System.Collections.Generic;       // List, IEnumerable…
using System.Threading.Tasks;           // Hỗ trợ async/await

namespace BusinessLayer.Service
{
    /// <summary>
    /// Service chịu trách nhiệm xử lý nghiệp vụ cho BloodUnit.
    /// </summary>
    public class BloodUnitService : IBloodUnitService
    {
        private readonly IBloodUnitRepository _bloodUnitRepository; // Repo truy vấn DB
        private readonly IMapper _mapper;                           // AutoMapper

        public BloodUnitService(IBloodUnitRepository bloodUnitRepository, IMapper mapper)
        {
            _bloodUnitRepository = bloodUnitRepository
                                    ?? throw new ArgumentNullException(nameof(bloodUnitRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Thêm đơn vị máu mới; mặc định BloodUnitStatusId = 1.
        /// </summary>
        public async Task AddBloodUnitAsync(BloodUnitDTO bloodUnit)
        {
            try
            {
                // Ánh xạ DTO → Entity
                var entity = _mapper.Map<BloodUnit>(bloodUnit);

                // 1 = trạng thái “mới nhập kho” (hard‑coded mặc định)
                entity.BloodUnitStatusId = 1;

                await _bloodUnitRepository.AddAsync(entity);   // Thêm vào DbContext
                await _bloodUnitRepository.SaveChangesAsync(); // Lưu DB
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding blood unit: {ex.Message}");
                throw; // Bubble up để controller/logging xử lý
            }
        }

        /// <summary>
        /// Xoá đơn vị máu theo ID.
        /// </summary>
        public Task<bool> DeleteBloodUnitAsync(int id)
        {
            try
            {
                // Tìm trước để đảm bảo tồn tại (GetByIdAsync trả về Task)
                var bloodUnit = _bloodUnitRepository.GetByIdAsync(id);
                if (bloodUnit == null)
                    throw new KeyNotFoundException("Blood unit not found.");

                _bloodUnitRepository.DeleteAsync(id);          // Đánh dấu xoá
                return _bloodUnitRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the blood unit.", ex);
            }
        }

        /// <summary> Lấy tất cả đơn vị máu. </summary>
        public async Task<IEnumerable<BloodUnit>> GetAllBloodUnitsAsync()
            => await _bloodUnitRepository.GetAllAsync();

        /// <summary> Lấy đơn vị máu theo ID hoặc ném lỗi nếu không tìm thấy. </summary>
        public async Task<BloodUnit> GetBloodUnitByIdAsync(int id)
            => await _bloodUnitRepository.GetByIdAsync(id)
               ?? throw new KeyNotFoundException("Blood unit not found.");

        /// <summary> Lấy danh sách đơn vị máu theo thành phần (hồng cầu, huyết tương…). </summary>
        public async Task<IEnumerable<BloodUnit>> GetBloodUnitsByBloodComponentAsync(int bloodComponentId)
            => await _bloodUnitRepository.GetUnitsByBloodComponentIdAsync(bloodComponentId);

        /// <summary> Lấy danh sách đơn vị máu theo nhóm máu. </summary>
        public async Task<IEnumerable<BloodUnit>> GetBloodUnitsByBloodTypeAsync(int bloodTypeId)
            => await _bloodUnitRepository.GetUnitsByBloodTypeIdAsync(bloodTypeId);

        /// <summary> Lấy danh sách đơn vị máu theo trạng thái (đủ điều kiện, đã sử dụng…). </summary>
        public async Task<IEnumerable<BloodUnit>> GetBloodUnitsByStatusAsync(int statusId)
            => await _bloodUnitRepository.GetUnitsByStatusAsync(statusId);

        /// <summary>
        /// Cập nhật thông tin đơn vị máu.
        /// </summary>
        public async Task<bool> UpdateBloodUnitAsync(BloodUnit bloodUnit)
        {
            bloodUnit.UpdatedAt = DateTime.UtcNow;  // Stamp thời gian cập nhật
            await _bloodUnitRepository.UpdateAsync(bloodUnit);
            await _bloodUnitRepository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cập nhật riêng trạng thái đơn vị máu (ví dụ: từ “kho” sang “đã cấp phát”).
        /// </summary>
        public Task<bool> UpdateBloodUnitStatusAsync(int unitId, int bloodUnitStatusId)
            => _bloodUnitRepository.UpdateUnitStatusAsync(unitId, bloodUnitStatusId);
    }
}

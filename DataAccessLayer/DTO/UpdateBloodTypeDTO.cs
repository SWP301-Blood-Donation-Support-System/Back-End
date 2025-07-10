using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTO
{
    public class UpdateBloodTypeDTO
    {
        [Required(ErrorMessage = "BloodTypeId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "BloodTypeId must be greater than 0")]
        public int BloodTypeId { get; set; }
    }

    public class UpdateUserBloodTypeByDonorIdDTO
    {
        [Required(ErrorMessage = "DonorId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "DonorId must be greater than 0")]
        public int DonorId { get; set; }
        
        [Required(ErrorMessage = "BloodTypeId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "BloodTypeId must be greater than 0")]
        public int BloodTypeId { get; set; }
    }
}
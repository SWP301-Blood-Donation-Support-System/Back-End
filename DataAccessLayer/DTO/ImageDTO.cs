using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTO
{
    /// <summary>
    /// Base DTO for image operations
    /// </summary>
    public class ImageDTO
    {
        [Required(ErrorMessage = "URL ảnh là bắt buộc")]
        [Url(ErrorMessage = "URL ảnh không hợp lệ")]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for storing a single image URL
    /// </summary>
    public class StoreImageUrlRequest
    {
        [Required(ErrorMessage = "URL ảnh là bắt buộc")]
        [Url(ErrorMessage = "URL ảnh không hợp lệ")]
        public string ImageUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for storing multiple image URLs
    /// </summary>
    public class StoreMultipleImageUrlsRequest
    {
        [Required(ErrorMessage = "Danh sách URL ảnh là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 URL ảnh")]
        [MaxLength(10, ErrorMessage = "Chỉ được phép lưu tối đa 10 URL ảnh cùng lúc")]
        public List<string> ImageUrls { get; set; } = new();
    }

    /// <summary>
    /// DTO for validating image URL
    /// </summary>
    public class ValidateImageUrlRequest
    {
        [Required(ErrorMessage = "URL ảnh là bắt buộc")]
        [Url(ErrorMessage = "URL ảnh không hợp lệ")]
        public string ImageUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for image URL storage operations
    /// </summary>
    public class ImageUrlStorageResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ImageStorageData? Data { get; set; }
    }

    /// <summary>
    /// Data contained in image storage response
    /// </summary>
    public class ImageStorageData
    {
        public string? ImageUrl { get; set; }
        public List<string>? ImageUrls { get; set; }
        public int? TotalUrls { get; set; }
        public int? ValidUrls { get; set; }
        public int? InvalidUrls { get; set; }
        public List<string>? FailedUrls { get; set; }
        public DateTime StoredAt { get; set; }
    }

    /// <summary>
    /// Response DTO for image URL validation
    /// </summary>
    public class ImageUrlValidationResponse
    {
        public string Status { get; set; } = string.Empty;
        public ImageValidationData? Data { get; set; }
    }

    /// <summary>
    /// Data contained in image validation response
    /// </summary>
    public class ImageValidationData
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public bool IsAccessible { get; set; }
        public DateTime CheckedAt { get; set; }
    }

    /// <summary>
    /// DTO for image metadata
    /// </summary>
    public class ImageMetadataDTO
    {
        public int? Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? OriginalFileName { get; set; }
        public long? FileSize { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? ContentType { get; set; }
        public int? UploadedByUserId { get; set; }
        public string? UploadedByUserName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// DTO for image upload information (for reference)
    /// </summary>
    public class ImageUploadInfoDTO
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? PublicId { get; set; }
        public string? Folder { get; set; }
        public List<string>? Tags { get; set; }
    }
}

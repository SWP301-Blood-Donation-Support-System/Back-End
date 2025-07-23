using Microsoft.AspNetCore.Http;

namespace BusinessLayer.IService
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
        Task<bool> DeleteImageAsync(string publicId);
        Task<bool> DeleteImageByUrlAsync(string imageUrl);
        Task<IEnumerable<string>> UploadMultipleImagesAsync(IEnumerable<IFormFile> imageFiles, string folder = null);
        string ExtractPublicIdFromUrl(string cloudinaryUrl);
        Task<string> GetOptimizedImageUrlAsync(string publicId, int? width = null, int? height = null, string quality = "auto");
    }
}
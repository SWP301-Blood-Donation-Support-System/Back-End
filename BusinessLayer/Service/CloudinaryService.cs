using BusinessLayer.IService;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _defaultFolder;

        public CloudinaryService(IConfiguration configuration)
        {
            // Try multiple configuration approaches
            //var cloudinaryUrl = configuration["Cloudinary:CloudinaryUrl"];
            
            //if (string.IsNullOrEmpty(cloudinaryUrl))
            //{
            //    // Fallback to individual configuration values
            //    var cloudName = configuration["Cloudinary:CloudName"];
            //    var apiKey = configuration["Cloudinary:ApiKey"];
            //    var apiSecret = configuration["Cloudinary:ApiSecret"];

            //    if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            //    {
            //        throw new ArgumentException("Cloudinary configuration is missing. Please provide either CloudinaryUrl or CloudName/ApiKey/ApiSecret");
            //    }

            //    var account = new Account(cloudName, apiKey, apiSecret);
            //    _cloudinary = new Cloudinary(account);
            //}
            //else
            //{
            //    _cloudinary = new Cloudinary(cloudinaryUrl);
            //}

            //_defaultFolder = configuration["Cloudinary:Folder"] ?? "blood-donation-system";
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("File ?nh không h?p l?");
            }

            // Ki?m tra ??nh d?ng file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Ch? h? tr? các ??nh d?ng ?nh: JPG, JPEG, PNG, GIF, BMP, WEBP");
            }

            // Ki?m tra kích th??c file (max 10MB)
            if (imageFile.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("Kích th??c file không ???c v??t quá 10MB");
            }

            try
            {
                using var stream = imageFile.OpenReadStream();
                
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, stream),
                    // Transform ?? resize n?u c?n (max width 1920px) v?i quality auto
                    Transformation = new Transformation()
                        .Width(1920)
                        .Crop("limit")
                        .Quality("auto")
                        .FetchFormat("auto"),
                    // T?o folder cho ?ng d?ng
                    Folder = _defaultFolder,
                    // Generate unique filename
                    PublicId = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}",
                    // Enable overwrite
                    Overwrite = false,
                    // Add tags for organization
                    Tags = "blood-donation,user-upload"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"L?i upload ?nh: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Không th? upload ?nh: {ex.Message}");
            }
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("File ?nh không h?p l?");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Ch? h? tr? các ??nh d?ng ?nh: JPG, JPEG, PNG, GIF, BMP, WEBP");
            }

            if (imageFile.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("Kích th??c file không ???c v??t quá 10MB");
            }

            try
            {
                using var stream = imageFile.OpenReadStream();
                
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, stream),
                    Transformation = new Transformation()
                        .Width(1920)
                        .Crop("limit")
                        .Quality("auto")
                        .FetchFormat("auto"),
                    Folder = !string.IsNullOrEmpty(folder) ? folder : _defaultFolder,
                    PublicId = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}",
                    Overwrite = false,
                    Tags = "blood-donation,user-upload"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"L?i upload ?nh: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Không th? upload ?nh: {ex.Message}");
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return false;
            }

            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                
                return result.Result == "ok";
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteImageByUrlAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return false;
            }

            var publicId = ExtractPublicIdFromUrl(imageUrl);
            if (string.IsNullOrEmpty(publicId))
            {
                return false;
            }

            return await DeleteImageAsync(publicId);
        }

        // Helper method ?? extract public ID t? Cloudinary URL
        public string ExtractPublicIdFromUrl(string cloudinaryUrl)
        {
            if (string.IsNullOrEmpty(cloudinaryUrl))
                return null;

            try
            {
                var uri = new Uri(cloudinaryUrl);
                var pathSegments = uri.AbsolutePath.Split('/');
                
                // Cloudinary URL format: /version/folder/publicId.extension
                // We need to get the publicId part
                var fileNameWithExtension = pathSegments.Last();
                var publicId = Path.GetFileNameWithoutExtension(fileNameWithExtension);
                
                // If there's a folder, include it
                if (pathSegments.Length > 3)
                {
                    var folder = pathSegments[pathSegments.Length - 2];
                    return $"{folder}/{publicId}";
                }
                
                return publicId;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetOptimizedImageUrlAsync(string publicId, int? width = null, int? height = null, string quality = "auto")
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return null;
            }

            try
            {
                var transformation = new Transformation()
                    .Quality(quality)
                    .FetchFormat("auto");

                if (width.HasValue)
                {
                    transformation = transformation.Width(width.Value);
                }

                if (height.HasValue)
                {
                    transformation = transformation.Height(height.Value);
                }

                if (width.HasValue || height.HasValue)
                {
                    transformation = transformation.Crop("fill");
                }

                var url = _cloudinary.Api.UrlImgUp.Transform(transformation).BuildUrl(publicId);
                return url;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<string>> UploadMultipleImagesAsync(IEnumerable<IFormFile> imageFiles, string folder = null)
        {
            var uploadedUrls = new List<string>();
            
            foreach (var file in imageFiles)
            {
                try
                {
                    var url = await UploadImageAsync(file, folder);
                    uploadedUrls.Add(url);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other files
                    Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                }
            }

            return uploadedUrls;
        }
    }
}
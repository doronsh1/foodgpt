using FoodAi.Persistence.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodAi.ApiService
{
    public class StorageService(AzureBlobStorageService _blobStorageService, ILogger<StorageService> _logger)
    {
        public async Task<string> UploadImageAsync(string userId, DateTimeOffset createdAt, string base64Image)
        {
            string fileName = $"{userId}_{createdAt.ToString("yyyyMMddHHmmss")}.jpg";
            string base64 = base64Image;
            if (base64Image.Contains(","))
            {
                base64 = base64Image.Substring(base64Image.IndexOf(",") + 1);
            }

            // Convert Base64 string to byte array

            var imageData = Convert.FromBase64String(base64);

            string url = "";
            using (var memoryStream = new MemoryStream(imageData))
            {
                url = await _blobStorageService.UploadImageAsync(fileName, memoryStream);
            }
            _logger.LogInformation($"Image uploaded to {url}");
            return fileName;

        }
    }
    
}

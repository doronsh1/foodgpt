using Azure.Storage.Blobs;
using FoodAi.Persistence.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.Persistence.Services
{
    public class AzureBlobStorageService
    {
        private readonly string _storageConnectionString;
        private readonly string _containerName;
        public AzureBlobStorageService(IOptions<AzureStorageSettings> options)
        {
            _storageConnectionString = options.Value.ConnectionString;
            _containerName = options.Value.ContainerName;
        }
        public async Task<string> UploadImageAsync(string fileName, Stream imageStream)
        {
            try
            {
                // Create a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConnectionString);

                // Create the container and return a container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                // Ensure the container exists
                await containerClient.CreateIfNotExistsAsync();

                // Get a reference to a blob            
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                //Console.WriteLine($"Uploading to Blob storage as blob: {fileName}");

                // upload file data
                await blobClient.UploadAsync(imageStream, true);


                // Return the URI of the uploaded blob
                return blobClient.Uri.ToString();
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
                         
        }

        public async Task<byte[]> DownloadImageAsync(string fileName)
        {
            try
            {
                // Create a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConnectionString);

                // Create the container and return a container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                // Ensure the container exists
                await containerClient.CreateIfNotExistsAsync();

                // Get a reference to a blob            
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                //Console.WriteLine($"Downloading from Blob storage as blob: {fileName}");

                // download file data
                var response = await blobClient.DownloadAsync();

                using (var memoryStream = new MemoryStream())
                {
                    await response.Value.Content.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
}
    }
}

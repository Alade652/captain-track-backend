using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CaptainTrackBackend.Application.Services.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private readonly Cloudinary _cloudinary;



        public FileUploadService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "uploads"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();

            throw new Exception("File upload failed");
        }
    }

    
}

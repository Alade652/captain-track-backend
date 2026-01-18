using Microsoft.AspNetCore.Http;

namespace CaptainTrackBackend.Application.Services.FileUpload
{
    public interface IFileUploadService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}

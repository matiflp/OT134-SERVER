using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadFile(string key, IFormFile file);
        Task<FileStreamResult> GetFile([FromQuery] string imageName);
        Task<string> AwsDeleteFile([FromQuery] string imageName);
    }
}

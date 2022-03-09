using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OngProject.Core.Helper;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface IS3AwsHelper
    {
        Task<AwsManagerResponse> AwsUploadFile(string key, IFormFile file);
        Task<FileStreamResult> AwsGetFile([FromQuery] string imageName);
        Task<AwsManagerResponse> AwsDeleteFile([FromQuery] string imageName);
    }
}

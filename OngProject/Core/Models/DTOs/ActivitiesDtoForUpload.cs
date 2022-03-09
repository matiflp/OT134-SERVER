using Microsoft.AspNetCore.Http;

namespace OngProject.Core.Models.DTOs
{
    public class ActivitiesDtoForUpload
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public IFormFile Image { get; set; }
    }
}
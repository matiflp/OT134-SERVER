using Microsoft.AspNetCore.Http;

namespace OngProject.Core.Models.DTOs
{
    public class NewDtoForUpload
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public IFormFile Image { get; set; }
        public int Category { get; set; }
    }
}
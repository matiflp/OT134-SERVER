using Microsoft.AspNetCore.Http;

namespace OngProject.Core.Models.DTOs
{
    public class MembersDtoForUpload
    {
        public string Name { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
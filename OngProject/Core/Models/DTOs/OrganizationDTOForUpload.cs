using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OngProject.Core.Models.DTOs
{
    public class OrganizationDTOForUpload
    {
        [Required(ErrorMessage = "The Name Is Required")]
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string Address { get; set; }
        public int? Phone { get; set; }
        public string Email { get; set; }
        public string WelcomeText { get; set; }
        public string AboutUsText { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedinUrl { get; set; }
    }
}
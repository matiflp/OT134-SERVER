using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OngProject.Core.Models.DTOs
{
    public class CategoryDTOForUpload
    {
        [Required(ErrorMessage = "The Name Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Name Is Too Long")]
        public string Name { get; set; }
        //permite valores nulos
        [StringLength(maximumLength: 255, ErrorMessage = "The Description Is Too Long")]
        public string Description { get; set; }
        //permite valores nulos        
        public IFormFile Image { get; set; }
    }
}

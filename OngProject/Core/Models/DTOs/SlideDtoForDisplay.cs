using System.ComponentModel.DataAnnotations;

namespace OngProject.Core.Models.DTOs
{
    public class SlideDtoForDisplay
    {
        [Display(Name = "UrlDeImagen")]
        public string ImageUrl { get; set; }

        [Display(Name = "Orden")]
        public int Order { get; set; }
    }
}

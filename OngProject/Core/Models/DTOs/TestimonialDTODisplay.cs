using System.ComponentModel.DataAnnotations;


namespace OngProject.Core.Models.DTOs
{
    public class TestimonialDTODisplay
    {
        [Required(ErrorMessage = "The name is required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The name is too long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The content is required")]
        [StringLength(maximumLength: 65535, ErrorMessage = "The content is too long")]
        public string Content { get; set; }

        [Required(ErrorMessage = "The image is required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The image is too long")]
        public string Image { get; set; }
    }
}

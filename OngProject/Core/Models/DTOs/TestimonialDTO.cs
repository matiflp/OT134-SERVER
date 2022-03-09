using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OngProject.Core.Models.DTOs
{
    public class TestimonialDTO
    {
        /// <summary>
        /// Name of the user who writes the testimonial.
        /// </summary>
        [Required(ErrorMessage = "The name is required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The name is too long")]
        public string Name { get; set; }

        /// <summary>
        /// Testimonial from a user.
        /// </summary>
        [Required(ErrorMessage = "The content is required")]
        [StringLength(maximumLength: 65535, ErrorMessage = "The content is too long")]
        public string Content { get; set; }

        /// <summary>
        /// Image accompanying the testimony
        /// </summary>
        [Required(ErrorMessage = "The image is required")]
        public IFormFile File { get; set; }
    }
}
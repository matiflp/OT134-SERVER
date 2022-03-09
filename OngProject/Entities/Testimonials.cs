using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OngProject.Entities
{
    [Table("testimonials")]
    public class Testimonials : EntityBase
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
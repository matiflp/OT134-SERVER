using System.ComponentModel.DataAnnotations;

namespace OngProject.Core.Models.DTOs
{
    public class CategoryDTOForRegister
    {
        /// <summary>
        /// Category name.
        /// </summary>
        [Required]
        public string Name { get; set; }
        [Required]

        /// <summary>
        /// Category description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category image file.
        /// </summary>
        [Required]
        public Microsoft.AspNetCore.Http.IFormFile Image { get; set; }
    }
}

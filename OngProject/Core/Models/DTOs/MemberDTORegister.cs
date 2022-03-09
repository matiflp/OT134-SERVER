using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OngProject.Core.Models.DTOs
{
    public class MemberDTORegister
    {
        /// <summary>
        /// Name of the member.
        /// </summary>
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(maximumLength: 255, ErrorMessage = "El nombre es demasiado largo")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the member.
        /// </summary>
        [Display(Name = "Descripción")]
        [StringLength(maximumLength: 255, ErrorMessage = "La descripción es demasiado larga")]
        public string Description { get; set; }

        /// <summary>
        /// Image of the member.
        /// </summary>
        [Required]
        public IFormFile File { get; set; }
    }
}
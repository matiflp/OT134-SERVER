using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OngProject.Core.Models.DTOs
{
    public class MemberDTODisplay
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(maximumLength: 255, ErrorMessage = "El nombre es demasiado largo")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(maximumLength: 255, ErrorMessage = "La descripción es demasiado larga")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The Image Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Image Is Too Long")]
        public string Image { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedinUrl { get; set; }
    }
}
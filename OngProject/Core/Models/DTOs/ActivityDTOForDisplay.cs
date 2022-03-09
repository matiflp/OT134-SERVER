using System.ComponentModel.DataAnnotations;

namespace OngProject.Core.Models.DTOs
{
    public class ActivityDTOForDisplay
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Nombre es requerido.")]
        [StringLength(255)]
        public string Name { get; set; }

        [Display(Name = "Contenido")]
        [Required(ErrorMessage = "Contenido es requerido.")]
        [StringLength(65535)]
        public string Content { get; set; }

        [Display(Name = "Imagen")]
        [Required(ErrorMessage = "Imagen es requerida.")]
        [StringLength(255)]
        public string Image { get; set; }
    }
}

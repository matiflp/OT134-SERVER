using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OngProject.Entities
{
    public class New: EntityBase
    {
        [Required(ErrorMessage = "Se requiere el nombre")]
        [StringLength(maximumLength: 255, ErrorMessage = "El nombre es demasiado largo (255 caracteres max)")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Se requiere el contenido")]
        [StringLength(maximumLength: 65535, ErrorMessage = "El contenido es demasiado largo (65535 caracteres max)")]
        public string Content { get; set; }
        
        [Required(ErrorMessage = "Se requiere la imagen")]
        [StringLength(maximumLength: 255, ErrorMessage = "El nombre de la imagen es demasiado largo (255 caracteres max)")]
        public string Image { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }        

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
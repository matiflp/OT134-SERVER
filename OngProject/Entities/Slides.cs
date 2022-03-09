using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OngProject.Entities
{
    public class Slides : EntityBase
    {
        [Display(Name = "UrlDeImagen")]
        public string ImageUrl { get; set; }

        [Display(Name = "Texto")]
        [StringLength(2000)]
        public string Text { get; set; }

        [Display(Name = "Orden")]
        [StringLength(255)]
        public int Order { get; set; }

        [Display(Name = "IdDeOrganizacion")]
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}

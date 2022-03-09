using System.ComponentModel.DataAnnotations;

namespace OngProject.Entities
{
    public class Contacts : EntityBase
    {
        [Display(Name = "Nombre")]
        [StringLength(255)]
        public string Name { get; set; }
        
        [Display(Name = "Telefono")]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(320)]
        public string Email { get; set; }

        [Display(Name = "Mensaje")]
        [StringLength(2000)]
        public string Message { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace OngProject.Entities
{
    public class Category : EntityBase
    {
        [Required(ErrorMessage = "The Name Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Name Is Too Long")]
        public string Name { get; set; }
        //permite valores nulos
        [StringLength(maximumLength: 255, ErrorMessage = "The Description Is Too Long")]
        public string Description { get; set; }
        //permite valores nulos
        [StringLength(maximumLength: 255, ErrorMessage = "The Name Of Image Is Too Long")]
        public string Image { get; set; }
    }
}

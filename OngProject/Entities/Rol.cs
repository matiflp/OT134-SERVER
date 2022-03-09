using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OngProject.Entities
{
    public class Rol : EntityBase
    {
        [Required(ErrorMessage = "The Name Is Required")]
        [StringLength(maximumLength:255, ErrorMessage = "The Name Is Too Long")]
        public string Name { get; set; }
        [StringLength(maximumLength: 255, ErrorMessage = "The Description Is Too Long")]
        public string Description { get; set; }
    }
}

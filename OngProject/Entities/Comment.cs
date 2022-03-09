using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OngProject.Entities
{
    public class Comment : EntityBase
    {
        public int NewId { get; set; }
        public virtual New New { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required(ErrorMessage = "Body es requerido.")]
        [StringLength(maximumLength: 65535, ErrorMessage = "Body es demasiado largo.")]
        public string Body { get; set; }
    }
}

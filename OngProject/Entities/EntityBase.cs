using System;
using System.ComponentModel.DataAnnotations;

namespace OngProject.Entities
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
        public DateTime LastModified { get; set; }
        public bool SoftDelete { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace OngProject.Entities
{
    public class Member : EntityBase
    {
        [Required(ErrorMessage = "The Name Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Name Is Too Long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Image Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Image Is Too Long")]
        public string Image { get; set; }

        [StringLength(maximumLength: 255, ErrorMessage = "The Url Link Is Too Long")]
        public string FacebookUrl { get; set; }

        [StringLength(maximumLength: 255, ErrorMessage = "The Url Link Is Too Long")]
        public string InstagramUrl { get; set; }

        [StringLength(maximumLength: 255, ErrorMessage = "The Url Link Is Too Long")]
        public string LinkedinUrl { get; set; }

        [StringLength(maximumLength: 255, ErrorMessage = "The Description Is Too Long")]
        public string Description { get; set; }
    }
}
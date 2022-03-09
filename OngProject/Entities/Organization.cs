using System.ComponentModel.DataAnnotations;

namespace OngProject.Entities
{
    public class Organization : EntityBase
    {
        [Required(ErrorMessage = "The Name Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Name Is Too Long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Image Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Image Is Too Long")]
        public string Image { get; set; }

        [StringLength(maximumLength: 255, ErrorMessage = "The Address Is Too Long")]
        public string Address { get; set; }

        [StringLength(maximumLength: 20, ErrorMessage = "The Phone Number Is Too Long")]
        public int? Phone { get; set; }

        [Required(ErrorMessage = "The Email Is Required")]
        [StringLength(maximumLength: 320, ErrorMessage = "The Email Is Too Long")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Welcome Text Is Required")]
        [StringLength(maximumLength: 500, ErrorMessage = "The Welcome Text Is Too Long")]
        public string WelcomeText { get; set; }

        [Required(ErrorMessage = "The About Us Text Is Required")]
        [StringLength(maximumLength: 2000, ErrorMessage = "The About Us Text Is Too Long")]
        public string AboutUsText { get; set; }

        [Url]
        [Required(ErrorMessage = "The Facebook Url Is Required")]
        public string FacebookUrl { get; set; }

        [Url]
        [Required(ErrorMessage = "The Instagram Url Is Required")]
        public string InstagramUrl { get; set; }

        [Url]
        [Required(ErrorMessage = "The Linkedin Url Is Required")]
        public string LinkedinUrl { get; set; }

    }

}

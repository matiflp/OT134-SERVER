using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OngProject.Core.Models.DTOs
{
    public class UserRegisterDto
    {
        /// <summary>
        ///     First name of the user.
        /// </summary>
        [Required(ErrorMessage = "The First Name Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The First Name Is Too Long")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of the user.
        /// </summary>
        [Required(ErrorMessage = "The Last Name Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Last Name Is Too Long")]
        public string LastName { get; set; }

        /// <summary>
        ///     Email addres to register.
        /// </summary>
        [Required(ErrorMessage = "The Email Is Required")]
        [StringLength(maximumLength: 320, ErrorMessage = "The Email Is Too Long")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]        
        public string Email { get; set; }

        /// <summary>
        ///     Password required to login.
        /// </summary>
        [Required(ErrorMessage = "The Password Is Required")]
        [StringLength(maximumLength: 255, ErrorMessage = "The Password Is Too Long")]
        public string Password { get; set; }

        /// <summary>
        ///     user profile picture.
        /// </summary>
        [Required(ErrorMessage = "The Image is required")]        
        public IFormFile Photo { get; set; }

        /// <summary>
        ///     Role Id that the user will have in the system.
        /// </summary>
        public int RolId { get; set; }
    }
}

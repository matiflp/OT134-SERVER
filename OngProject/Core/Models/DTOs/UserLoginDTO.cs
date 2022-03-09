using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OngProject.Core.Models.DTOs
{
    public class UserLoginDTO
    {
        /// <summary>
        ///     Email to login.
        /// </summary>
        [Required(ErrorMessage = "The Email Is Required")]
        [StringLength(maximumLength: 320, ErrorMessage = "The Email Is Too Long")]
        public string Email { get; set; }

        /// <summary>
        ///     Enter the password to login.
        /// </summary>
        [Required(ErrorMessage = "The Password Is Required")]
        [StringLength(maximumLength: 20, ErrorMessage = "The Password Is Too Long")]
        public string Password { get; set; }
    }
}

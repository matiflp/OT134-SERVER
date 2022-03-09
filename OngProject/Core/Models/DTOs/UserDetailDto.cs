
namespace OngProject.Core.Models.DTOs
{
    public class UserDetailDto
    {        
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public int RolId { get; set; }        
    }
}

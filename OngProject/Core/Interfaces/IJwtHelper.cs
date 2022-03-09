using OngProject.Entities;

namespace OngProject.Core.Interfaces
{
    public interface IJwtHelper
    {
        string GenerateJwtToken(User user);
    }
}

using OngProject.Core.Models.DTOs;
using System.Threading.Tasks;
using OngProject.Core.Models.Response;
using OngProject.Core.Models.PagedResourceParameters;

namespace OngProject.Core.Interfaces
{
    public interface IUserService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(UserRegisterDto dto);
        Task<Result> LoginAsync(UserLoginDTO userLoginDto);
        Task<Result> Update(int id, UserUpdateDto user);
        Task<Result> Delete(int id);
    }
}
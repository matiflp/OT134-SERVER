using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface ICommentsService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(CommentDtoForRegister commentDTO);
        Task<Result> Update(int idComment, CommentDtoForRegister commentDto);
        Task<Result> Delete(int IdComment);
    }
}
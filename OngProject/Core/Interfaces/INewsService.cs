using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface INewsService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(NewDtoForUpload newDTO);
        Task<Result> Update(int id, NewDtoForUpload newsDTO);
        Task<Result> Delete(int id);
    }
}

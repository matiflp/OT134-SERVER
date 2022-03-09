using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Response;
using OngProject.Entities;
using System.Threading.Tasks;
using OngProject.Core.Helper;
using OngProject.Core.Models.PagedResourceParameters;

namespace OngProject.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<Result> GetAll(PaginationParams pagingParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(CategoryDTOForRegister categoryDTO);
        Task<Result> Update(int id, CategoryDTOForUpload dto);
        Task<Result> Delete(int id);
    }
}

using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using OngProject.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface ISlideSerivice
    {
        Task<Result> GetAll(PaginationParams paginationParam);
        Task<ICollection<SlideDtoForDisplay>> GetAllByOrganization(int idOrganization);
        Task<Result> GetById(int id);
        Task<Result> Insert(SlideDtoForUpload slideDto);
        Task<Result> Update(int id, SlideDtoForUpdate dto);
        Task<Result> Delete(int id);
    }
}
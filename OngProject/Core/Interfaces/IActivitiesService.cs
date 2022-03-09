using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Response;
using System.Threading.Tasks;
using OngProject.Core.Models.PagedResourceParameters;

namespace OngProject.Core.Interfaces
{
    public interface IActivitiesService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(ActivityDTOForRegister activities);
        Task <Result> Update(int id, ActivitiesDtoForUpload activitiesDto);
        Task<Result> Delete(int id);
    }
}
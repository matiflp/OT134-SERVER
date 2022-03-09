using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface IOrganizationsService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(OrganizationDTOForUpload organizationDTOForUpload);
        Task<Result> Update(int id, OrganizationDTOForUpload organizationDTOForUpload);
        Task<Result> Delete(int id);
    }
}
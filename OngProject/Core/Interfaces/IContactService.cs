using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface IContactService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(ContactDTO contactDto);
        Task<Result> Update(int id, ContactDTO contactDto);
        Task<Result> Delete(int id);
    }
}

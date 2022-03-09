using OngProject.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Response;
using OngProject.Core.Models.PagedResourceParameters;

namespace OngProject.Core.Interfaces
{
    public interface IMemberService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(MemberDTORegister memberDTO);
        Task<Result> Update(int id, MembersDtoForUpload memberDTO);
        Task<Result> Delete(int id);
    }
}
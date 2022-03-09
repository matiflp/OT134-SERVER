using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface ITestimonialsService
    {
        Task<Result> GetAll(PaginationParams paginationParams);
        Task<Result> GetById(int id);
        Task<Result> Insert(TestimonialDTO testimonialDTO);
        Task<Result> Update(int id,TestimonialDTO testimonialDTO);
        Task<Result> Delete(int id);
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OngProject.Core.Helper;
using OngProject.Core.Interfaces;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Paged;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using OngProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OngProject.Core.Business
{
    public class TestimonialsService : ITestimonialsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEntityMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContext;

        public TestimonialsService(IUnitOfWork unitOfWork,
                          IConfiguration configuration,
                          IEntityMapper mapper,
                          IImageService imageService,
                          IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _config = configuration;
            _mapper = mapper;
            _imageService = imageService;
            _httpContext = httpContext;
        }

        public async Task<Result> GetAll(PaginationParams paginationParams)
        {
            try
            {
                var testimonials = await _unitOfWork.TestimonialsRepository.FindAllAsync(null, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.TestimonialsRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen testimonios", 404);

                if (testimonials.Count == 0)
                    return Result.FailureResult("Paginación inválida, no hay resultados", 400);

                var testimonialsDTOForDisplay = testimonials
                    .Where(newEntity => !newEntity.SoftDelete)
                    .Select(testimoial => _mapper.TestimonialDTOToTestimonialDisplay(testimoial));

                var paged = PagedList<TestimonialDTODisplay>.Create(testimonialsDTOForDisplay.ToList(), totalCount,
                                                      paginationParams.PageNumber, 
                                                      paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<TestimonialDTODisplay>(paged, url);

                return Result<PagedResponse<TestimonialDTODisplay>>.SuccessResult(pagedResponse);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message });
            }
        }

        public async Task<Result> GetById(int id)
        {
            try
            {
                var testimonial = await _unitOfWork.TestimonialsRepository.GetByIdAsync(id);

                if (testimonial is not null)
                {
                    if (testimonial.SoftDelete)
                        return Result.FailureResult($"id({testimonial.Id}) se encuentra eliminado del sistema.", 400);

                    var tetimonialDto = _mapper.TestimonialToTestimonialDTO(testimonial);
                    return Result<TestimonialDTO>.SuccessResult(tetimonialDto);
                }

                return Result.FailureResult("El testimonio no existe.", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Insert(TestimonialDTO testimonialDTO)
        {
            try
            {
                if (testimonialDTO is not null)
                {
                    var testimonial = _mapper.TestimonialDTOToTestimonial(testimonialDTO);

                    var resultName = await _unitOfWork.TestimonialsRepository.FindByConditionAsync(x => x.Name == testimonialDTO.Name);
                    if (resultName.Count > 0)
                        return Result.FailureResult("El nombre del testimonio ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    if (testimonialDTO.File is not null)
                        testimonial.Image = await _imageService.UploadFile($"{Guid.NewGuid()}{testimonialDTO.File.FileName}", testimonialDTO.File);

                    testimonial.LastModified = DateTime.Now;

                    await _unitOfWork.TestimonialsRepository.Create(testimonial);
                    await _unitOfWork.SaveChangesAsync();

                    var testimonialDisplay = _mapper.TestimonialDTOToTestimonialDisplay(testimonial);
                    return Result<TestimonialDTODisplay>.SuccessResult(testimonialDisplay);
                }

                return Result.FailureResult("Datos proporcionados erroneos", 400);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Testimonio no registrada: " + e.Message }, 500);
            }
        }

        public async Task<Result> Update(int id, TestimonialDTO dto)
        {
            try
            {
                var testimonial = await _unitOfWork.TestimonialsRepository.GetByIdAsync(id);

                if (testimonial is not null)
                {
                    if (testimonial.SoftDelete)
                        return Result.FailureResult("El testimonio se encuentra eliminado del sistema", 400);

                    var resultName = await _unitOfWork.TestimonialsRepository.FindByConditionAsync(x => x.Name == dto.Name);
                    if (resultName.Count > 0)
                        return Result.FailureResult("El nombre del testimonio ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    testimonial.Name = dto.Name;
                    testimonial.Content = dto.Content;
                    testimonial.LastModified = DateTime.Now;

                    if (dto.File is not null)
                    {
                        await _imageService.AwsDeleteFile(testimonial.Image[(testimonial.Image.LastIndexOf("/") + 1)..]);
                        testimonial.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{dto.File.FileName}", dto.File);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var testimonialDisplay = _mapper.TestimonialDTOToTestimonialDisplay(testimonial);
                    return Result<TestimonialDTODisplay>.SuccessResult(testimonialDisplay);
                }

                return Result.FailureResult("Id de Testimonio inexistente.", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message });
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                var testimonial = await _unitOfWork.TestimonialsRepository.GetByIdAsync(id);
                if (testimonial is not null)
                {
                    if (testimonial.SoftDelete)
                        return Result.FailureResult("El testimonio ya se encuentra eliminado del sistema", 400);

                    testimonial.SoftDelete = true;
                    testimonial.LastModified = DateTime.Now;

                    await this._unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult("Testimonio eliminado.");
                }

                return Result.FailureResult("No existe un testimonio con ese Id", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }
    }
}

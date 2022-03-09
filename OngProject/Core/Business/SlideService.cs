using Microsoft.AspNetCore.Http;
using OngProject.Core.Helper;
using OngProject.Core.Helper.formFile;
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
    public class SlideService : ISlideSerivice
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContext;

        public SlideService(IUnitOfWork unitOfWork, IEntityMapper mapper, IImageService imageService, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
            _httpContext = httpContext;
        }      

        public async Task<Result> GetAll(PaginationParams paginationParams)
        {
            try
            {
                var slides = await _unitOfWork.SlideRepository.FindAllAsync(x => !x.SoftDelete, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.ActivitiesRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen actividades", 404);

                if (slides.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados", 400);

                var slidessDTOForDisplay = slides
                    .Select(slide => _mapper.SlideToSlideDtoForDisplay(slide));

                var paged = PagedList<SlideDtoForDisplay>.Create(slidessDTOForDisplay.ToList(), totalCount,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<SlideDtoForDisplay>(paged, url);

                return Result<PagedResponse<SlideDtoForDisplay>>.SuccessResult(pagedResponse);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }

        public async Task<ICollection<SlideDtoForDisplay>> GetAllByOrganization(int idOrganization)
        {
            var slides = await _unitOfWork.SlideRepository.FindByConditionAsync(x => x.OrganizationId == idOrganization && !x.SoftDelete);
            if (slides.Count == 0)
                return null;

            List<SlideDtoForDisplay> dto = new();
            foreach (var item in slides)
                dto.Add(_mapper.SlideToSlideDtoForDisplay(item));

            return dto;       
        }

        public async Task<Result> GetById(int id)
        {
            try
            {
                var slide = await _unitOfWork.SlideRepository.GetByIdAsync(id);

                if (slide is not null)
                {
                    if (slide.SoftDelete)
                        return Result.FailureResult($"id({slide.Id}) se encuentra eliminado del sistema.", 400);

                    var slideDto = _mapper.SlideToSlideDtoForDisplay(slide);
                    return Result<SlideDtoForDisplay>.SuccessResult(slideDto);
                }

                return Result.FailureResult("El slide no existe.", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Insert(SlideDtoForUpload slideDto)
        {
            try
            {
                if (string.IsNullOrEmpty(slideDto.ImageUrl))
                    return Result.FailureResult("Se debe ingresar Imagen", 400);

                if (string.IsNullOrEmpty(slideDto.Text))
                    return Result.FailureResult("Se debe ingresar Texto", 400);

                if (slideDto.OrganizationId < 1)
                    return Result.FailureResult("Se debe ingresar Id para Organizacion entero y mayor a cero", 400);

                if (slideDto.Order < 0)
                    return Result.FailureResult("El numero de Orden debe ser mayor a cero", 400);

                if (slideDto.Order == 0)
                {
                    var slidesList = await _unitOfWork.SlideRepository.FindByConditionAsync(
                        x => x.OrganizationId == slideDto.OrganizationId);

                    if (slidesList is not null)
                    {
                        slidesList = slidesList.OrderBy(x => x.Order).ToList();
                        slideDto.Order = slidesList.LastOrDefault().Order + 1;
                    }
                    else
                        slideDto.Order = 1;
                }
                else
                {
                    var slidesList = await _unitOfWork.SlideRepository.FindByConditionAsync(
                        x => x.OrganizationId == slideDto.OrganizationId && x.Order == slideDto.Order);

                    if (slidesList.Count != 0)
                        return Result.FailureResult("Numero de Orden ya Ingresado, ingrese uno nuevo", 400);
                }

                var slide = _mapper.SlideDtoForUploadToSlide(slideDto);

                slide.ImageUrl = await UploadEncodedImageToBucketAsync(slideDto.ImageUrl);
                slide.LastModified = DateTime.Now;

                await _unitOfWork.SlideRepository.Create(slide);
                _unitOfWork.SaveChanges();

                var newSlideDto = _mapper.SlideToSlideDtoForDisplay(slide);
                return Result<SlideDtoForDisplay>.SuccessResult(newSlideDto);
            }
            catch (Exception ex)
            {
                return Result.FailureResult(ex.Message);
            }
        }

        public async Task<Result> Update(int id, SlideDtoForUpdate dto)
        {
            try
            {
                var slide = await _unitOfWork.SlideRepository.GetByIdAsync(id);

                if (slide is not null)
                {
                    if (slide.SoftDelete)
                        return Result.FailureResult("El slide seleccionado esta eliminado", 400);

                    if (string.IsNullOrEmpty(dto.ImageUrl))
                        return Result.FailureResult("Se debe ingresar Imagen", 400);

                    if (string.IsNullOrEmpty(dto.Text))
                        return Result.FailureResult("Se debe ingresar Texto", 400);

                    if (dto.Order < 0)
                        return Result.FailureResult("El numero de Orden debe ser mayor a cero", 400);

                    if (dto.Order == 0)
                        dto.Order = slide.Order;
                    else
                    {
                        var slidesList = await _unitOfWork.SlideRepository.FindByConditionAsync(
                            x => x.OrganizationId == slide.OrganizationId && x.Order == dto.Order);

                        if (slidesList.Count != 0)
                            return Result.FailureResult("Numero de Orden ya Ingresado, ingrese uno nuevo", 400);
                    }

                    slide.Text = dto.Text;
                    slide.Order = dto.Order;
                    slide.LastModified = DateTime.Now;

                    if (!string.IsNullOrEmpty(dto.ImageUrl))
                    {
                        await _imageService.AwsDeleteFile(slide.ImageUrl[(slide.ImageUrl.LastIndexOf("/") + 1)..]);
                        slide.ImageUrl = await UploadEncodedImageToBucketAsync(dto.ImageUrl);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var slideDto = _mapper.SlideToSlideDtoForDisplay(slide);
                    return Result<SlideDtoForDisplay>.SuccessResult(slideDto);
                }

                return Result.FailureResult("Id de Slide inexistente.", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                var slide = await _unitOfWork.SlideRepository.GetByIdAsync(id);

                if (slide is not null)
                {
                    if (slide.SoftDelete)
                        return Result.FailureResult("El slide seleccionado ya fue eliminada", 400);

                    slide.SoftDelete = true;
                    slide.LastModified = DateTime.Now;

                    await _unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult($"Slide con {id} eliminado exitosamente.");
                }

                return Result.FailureResult("El slide no existe.", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Error al eliminar el slide: " + e.Message }, 500);
            }
        }

        private async Task<string> UploadEncodedImageToBucketAsync(string rawBase64File)
        {
            string newName = $"{Guid.NewGuid()}_user";

            int indexOfSemiColon = rawBase64File.IndexOf(";", StringComparison.OrdinalIgnoreCase);
            string dataLabel = rawBase64File.Substring(0, indexOfSemiColon);
            string contentType = dataLabel.Split(':').Last();
            var startIndex = rawBase64File.IndexOf("base64,", StringComparison.OrdinalIgnoreCase) + 7;
            var fileContents = rawBase64File.Substring(startIndex);

            var formFileData = new FormFileData()
            {
                FileName = newName,
                ContentType = contentType,
                Name = newName
            };

            byte[] imageBinaryFile = Convert.FromBase64String(fileContents);
            IFormFile newFile = ConvertFile.BinaryToFormFile(imageBinaryFile, formFileData);
            return await _imageService.UploadFile(newFile.FileName, newFile);
        }
    }
}

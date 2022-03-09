using Microsoft.AspNetCore.Http;
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
using Microsoft.Extensions.Configuration;

namespace OngProject.Core.Business
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEntityMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContext;

        public NewsService(IUnitOfWork unitOfWork,
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
                var news = await _unitOfWork.NewsRepository.FindAllAsync(null, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.NewsRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen noticias", 404);

                if (news.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados", 404);

                var newsDTOForDisplay = news
                    .Where(newEntity => !newEntity.SoftDelete)
                    .Select(newEntity => _mapper.NewtoNewDtoForDisplay(newEntity));

                var paged = PagedList<NewDtoForDisplay>.Create(newsDTOForDisplay.ToList(), totalCount,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<NewDtoForDisplay>(paged, url);

                return Result<PagedResponse<NewDtoForDisplay>>.SuccessResult(pagedResponse);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }

        public async Task<Result> GetById(int id)
        {
            try
            {
                var newEntity = await this._unitOfWork.NewsRepository.GetByIdAsync(id);

                if (newEntity is not null)
                {
                    if (newEntity.SoftDelete)
                        return Result.FailureResult($"id({newEntity.Id}) se encuentra eliminada del sistema.", 400);
                    
                    var newToNewDtoForDisplay = _mapper.NewtoNewDtoForDisplay(newEntity);
                    return Result<NewDtoForDisplay>.SuccessResult(newToNewDtoForDisplay);
                }

                return Result.FailureResult("id de noticia inexistente.", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }

        public async Task<Result> Insert(NewDtoForUpload newDtoForUpload)
        {
            try
            {
                if (newDtoForUpload is not null)
                {
                    var ValidationName = await _unitOfWork.NewsRepository.FindByConditionAsync(x => x.Name == newDtoForUpload.Name);
                    if (ValidationName.Count > 0)
                        return Result.FailureResult("Una noticia con ese nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    var ValidationContent = await _unitOfWork.NewsRepository.FindByConditionAsync(x => x.Content == newDtoForUpload.Content);
                    if (ValidationContent.Count > 0)
                        return Result.FailureResult("Una noticia con ese contenido ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    var newEntity = _mapper.NewDtoForUploadtoNew(newDtoForUpload);

                    if (newDtoForUpload.Image is not null)
                        newEntity.Image = await _imageService.UploadFile(newDtoForUpload.Image.FileName, newDtoForUpload.Image);

                    newEntity.LastModified = DateTime.Today;

                    await _unitOfWork.NewsRepository.Create(newEntity);
                    await _unitOfWork.SaveChangesAsync();

                    var newtoNewDtoForDisplay = _mapper.NewtoNewDtoForDisplay(newEntity);

                    return Result<NewDtoForDisplay>.SuccessResult(newtoNewDtoForDisplay);
                }

                return Result.FailureResult("Datos proporcionados erroneos", 400);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Novedad no registrada: " + e.Message }, 500);
            }
        }

        public async Task<Result> Update(int id, NewDtoForUpload newsDTO)
        {
            try
            {
                var news = await _unitOfWork.NewsRepository.GetByIdAsync(id);

                if (news is not null)
                {
                    if (news.SoftDelete)
                        return Result.FailureResult($"id({news.Id}) esta eliminado del sistema.", 400);

                    var ValidationName = await _unitOfWork.NewsRepository.FindByConditionAsync(x => x.Name == newsDTO.Name);
                    if (ValidationName.Count > 0)
                        return Result.FailureResult("Una noticia con ese nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    var ValidationContent = await _unitOfWork.NewsRepository.FindByConditionAsync(x => x.Content == newsDTO.Content);
                    if (ValidationContent.Count > 0)
                        return Result.FailureResult("Una noticia con ese contenido ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    news.Name = newsDTO.Name;
                    news.Content = newsDTO.Content;
                    news.CategoryId = newsDTO.Category;
                    news.LastModified = DateTime.Now;

                    if (newsDTO.Image is not null)
                    {
                        await _imageService.AwsDeleteFile(news.Image[(news.Image.LastIndexOf("/") + 1)..]);
                        news.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{newsDTO.Image.FileName}", newsDTO.Image);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var newsDisplay = _mapper.NewtoNewDtoForDisplay(news);
                    return Result<NewDtoForDisplay>.SuccessResult(newsDisplay);
                }
                    
                return Result.FailureResult("Id de noticia inexistente.", 404);
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
                var newEntity = await this._unitOfWork.NewsRepository.GetByIdAsync(id);

                if (newEntity is not null)
                {
                    if (newEntity.SoftDelete)
                        return Result.FailureResult($"id({newEntity.Id}) ya esta eliminado del sistema.", 400);
                    
                    newEntity.SoftDelete = true;
                    newEntity.LastModified = DateTime.Today;

                    await this._unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult($"Noticia:({newEntity.Id}) ha sido eliminada exitosamente.");
                }

                return Result.FailureResult("id de noticia inexistente.", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }
    }
}
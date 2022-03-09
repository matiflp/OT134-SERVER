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


namespace OngProject.Core.Business
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _entityMapper;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContext;

        public CategoryService(IUnitOfWork unitOfWork, IImageService imageService, IEntityMapper entityMapper, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _entityMapper = entityMapper;
            _httpContext = httpContext;
        }
       
        public async Task<Result> GetAll(PaginationParams paginationParams)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.FindAllAsync(null, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.CategoryRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen categorias", 404);

                if (categories.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados", 400);

                var categoriesDTOForDisplay = categories
                    .Where(newEntity => !newEntity.SoftDelete)
                    .Select(category => _entityMapper.CategoryToCategoryDtoForDisplay(category));

                var paged =  PagedList<CategoryDtoForDisplay>.Create(categoriesDTOForDisplay.ToList(), totalCount,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<CategoryDtoForDisplay>(paged, url);
                
                return Result<PagedResponse<CategoryDtoForDisplay>>.SuccessResult(pagedResponse);
            }
            catch(Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }

        public async Task<Result> GetById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category is not null)
                {
                    if (category.SoftDelete)
                        return Result.FailureResult($"id({category.Id}) se encuentra eliminada del sistema.", 400);

                    var categoryDto = _entityMapper.CategoryToCategoryDTO(category);
                    return Result<CategoryDTO>.SuccessResult(categoryDto);
                }

                return Result.FailureResult("La categoria no existe.", 404);

            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Insert(CategoryDTOForRegister categoryDTO)
        {
            try
            {
                if (categoryDTO is not null)
                {
                    var newCategory = this._entityMapper.CategoryDtoForRegisterToCategory(categoryDTO);

                    if (categoryDTO.Image is not null)
                        newCategory.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{categoryDTO.Image.FileName}", categoryDTO.Image);
                    
                    newCategory.LastModified = DateTime.Now;

                    await _unitOfWork.CategoryRepository.Create(newCategory);
                    await _unitOfWork.SaveChangesAsync();

                    return Result<CategoryDTO>.SuccessResult(_entityMapper.CategoryToCategoryDTO(newCategory));
                }

                return Result.FailureResult("Datos proporcionados erroneos.", 400);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Ocurrio un problema al crear una nueva categoria: " + e.Message }, 500);
            }
        }

        public async Task<Result> Update(int id, CategoryDTOForUpload dto)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category is not null)
                {
                    if (category.SoftDelete)
                        return Result.FailureResult("La categoria seleccionada esta eliminada", 400);

                    category.Name = dto.Name;
                    category.Description = dto.Description;
                    category.LastModified = DateTime.Now;

                    if (dto.Image is not null)
                    {
                        await _imageService.AwsDeleteFile(category.Image[(category.Image.LastIndexOf("/") + 1)..]);
                        category.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{dto.Image.FileName}", dto.Image);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var categoryDto = _entityMapper.CategoryToCategoryDTO(category);
                    return Result<CategoryDTO>.SuccessResult(categoryDto);
                }

                return Result.FailureResult("Id de categoria inexistente.", 404);
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
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category is not null)
                {
                    if (category.SoftDelete)
                        return Result.FailureResult("La categoria seleccionada ya fue eliminada", 400);

                    category.SoftDelete = true;
                    category.LastModified = DateTime.Now;

                    await _unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult($"Categoria {id} eliminada exitosamente.");
                }

                return Result.FailureResult("La categoria no existe.", StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Error al eliminar la categoria: " + e.Message });
            }
        }
    }
}

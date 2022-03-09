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
    public class ActivitiesService : IActivitiesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContext;

        public ActivitiesService(IUnitOfWork unitOfWork, IEntityMapper mapper, IImageService imageService, IHttpContextAccessor httpContext)
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
                var activities = await _unitOfWork.ActivitiesRepository.FindAllAsync(null, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.ActivitiesRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen actividades", 404);

                if (activities.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados", 400);
                
                var activitiesDTOForDisplay = activities
                    .Where(newEntity => !newEntity.SoftDelete)
                    .Select(activitie => _mapper.ActivityForActivityDTODisplay(activitie));

                var paged = PagedList<ActivityDTOForDisplay>.Create(activitiesDTOForDisplay.ToList(), totalCount,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<ActivityDTOForDisplay>(paged, url);

                return Result<PagedResponse<ActivityDTOForDisplay>>.SuccessResult(pagedResponse);
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
                var activity = await _unitOfWork.ActivitiesRepository.GetByIdAsync(id);

                if (activity is not null)
                {
                    if (activity.SoftDelete)
                        return Result.FailureResult($"id({activity.Id}) se encuentra eliminado del sistema.", 400);

                    var activityDto = _mapper.ActivityForActivityDTODisplay(activity);
                    return Result<ActivityDTOForDisplay>.SuccessResult(activityDto);
                }

                return Result.FailureResult("La actividad no existe.", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Insert(ActivityDTOForRegister activities)
        {
            try
            {
                if(activities is not null)
                {
                    var activity = _mapper.ActivityDTOForRegister(activities);

                    var resultName = await this._unitOfWork.ActivitiesRepository.FindByConditionAsync(x => x.Name == activities.Name);
                    var resultContent = await this._unitOfWork.ActivitiesRepository.FindByConditionAsync(x => x.Content == activities.Content);

                    if (resultName.Count > 0)
                        return Result.FailureResult("El Nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    if (resultContent.Count > 0)
                        return Result.FailureResult("El Contenido ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    if (activities.file is not null)
                        activity.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{activities.file.FileName}", activities.file);

                    activity.LastModified = DateTime.Today;

                    await _unitOfWork.ActivitiesRepository.Create(activity);
                    await _unitOfWork.SaveChangesAsync();

                    var obj = _mapper.ActivityForActivityDTODisplay(activity);
                    return Result<ActivityDTOForDisplay>.SuccessResult(obj);
                }

                return Result.FailureResult("Datos proporcionados erroneos", 400);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Actividad no registrada: " + e.Message }, 500);
            }
        }       

        public async Task<Result> Update(int id, ActivitiesDtoForUpload activitiesDto)
        {
            try
            {
                var activity = await _unitOfWork.ActivitiesRepository.GetByIdAsync(id);

                if (activity is not null)
                {
                    if (activity.SoftDelete)
                        return Result.FailureResult("La actividad seleccionada esta eliminada", 400);

                    var resultName = await this._unitOfWork.ActivitiesRepository.FindByConditionAsync(x => x.Name == activitiesDto.Name);
                    if (resultName.Count > 0)
                        return Result.FailureResult("El Nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);
                    
                    var resultContent = await this._unitOfWork.ActivitiesRepository.FindByConditionAsync(x => x.Content == activitiesDto.Content);
                    if (resultContent.Count > 0)
                        return Result.FailureResult("El Contenido ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    activity.Name = activitiesDto.Name;
                    activity.Content = activitiesDto.Content;
                    activity.LastModified = DateTime.Now;

                    if (activitiesDto.Image is not null)
                    {
                        await _imageService.AwsDeleteFile(activity.Image[(activity.Image.LastIndexOf("/") + 1)..]);
                        activity.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{activitiesDto.Image.FileName}", activitiesDto.Image);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var activitiesDisplay = _mapper.ActivityForActivityDTODisplay(activity);
                    return Result<ActivityDTOForDisplay>.SuccessResult(activitiesDisplay);
                }

                return Result.FailureResult("No se encontro el id", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Error al actualizar la activity: " + e.Message }, 500);
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                var activity = await _unitOfWork.ActivitiesRepository.GetByIdAsync(id);

                if (activity is not null)
                {
                    if (activity.SoftDelete)
                        return Result.FailureResult("La actividad seleccionada ya fue eliminada", 400);

                    activity.SoftDelete = true;
                    activity.LastModified = DateTime.Now;

                    await _unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult($"Actividad {id} eliminada exitosamente.");
                }

                return Result.FailureResult("La actividad no existe.", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Error al eliminar la activity: " + e.Message }, 500);
            }
        }
    }
}
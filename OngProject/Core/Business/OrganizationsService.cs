using OngProject.Core.Interfaces;
using OngProject.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Response;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using OngProject.Core.Helper;
using OngProject.Core.Models.PagedResourceParameters;
using Microsoft.AspNetCore.Http;
using OngProject.Core.Models.Paged;
using System.Linq;

namespace OngProject.Core.Business
{
    public class OrganizationService : IOrganizationsService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IJwtHelper _jwtHelper;
        private readonly ISlideSerivice _slideSerivice;
        private readonly IHttpContextAccessor _httpContext;

        public OrganizationService(IUnitOfWork unitOfWork, IConfiguration configuration, IJwtHelper jwtHelper, IEntityMapper mapper, ISlideSerivice slideSerivice, IImageService imageService, IHttpContextAccessor httpContext)
        {
            _config = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slideSerivice = slideSerivice;
            _jwtHelper = jwtHelper;
            _imageService = imageService;
            _httpContext = httpContext;
        }

        public async Task<Result> GetAll(PaginationParams paginationParams)
        {
            try
            {
                var organizations = await _unitOfWork.OrganizationRepository.FindAllAsync(org => !org.SoftDelete, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.OrganizationRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen organizaciones", 404);

                if (organizations.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados", 400);

                List<OrganizationDTOForDisplay> organizationDtoForDisplay = new();

                foreach (var entity in organizations)
                {
                    var orgDto = _mapper.OrganizationToOrganizationDTOForDisplay(entity);

                    orgDto.Slides = await _slideSerivice.GetAllByOrganization(entity.Id);

                    organizationDtoForDisplay.Add(orgDto);
                }

                
                var paged = PagedList<OrganizationDTOForDisplay>.Create(organizationDtoForDisplay.ToList(), totalCount,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<OrganizationDTOForDisplay>(paged, url);

                return Result<PagedResponse<OrganizationDTOForDisplay>>.SuccessResult(pagedResponse);
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
                var organization = await this._unitOfWork.OrganizationRepository.GetByIdAsync(id);

                if (organization is not null)
                {
                    if (organization.SoftDelete)
                        return Result.FailureResult($"id({organization.Id}) se encuentra eliminada del sistema.", 400);

                    var orgToOrgDtoForDisplay = _mapper.OrganizationToOrganizationDTOForDisplay(organization);
                    orgToOrgDtoForDisplay.Slides = await _slideSerivice.GetAllByOrganization(id);

                    return Result<OrganizationDTOForDisplay>.SuccessResult(orgToOrgDtoForDisplay);
                }

                return Result.FailureResult("id de organizacion inexistente.", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }

        public async Task<Result> Insert(OrganizationDTOForUpload organizationDTOForUpload)
        {
            try
            {
                if (organizationDTOForUpload is not null) 
                {
                    var ValidationName = await _unitOfWork.OrganizationRepository.FindByConditionAsync(x => x.Name == organizationDTOForUpload.Name);
                    if (ValidationName.Count > 0)
                        return Result.FailureResult("Una organizacion con ese nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    var organization = _mapper.OrganizationDtoForUploadtoOrganization(organizationDTOForUpload);

                    if (organizationDTOForUpload is not null)
                        organization.Image = await _imageService.UploadFile(organizationDTOForUpload.Image.FileName, organizationDTOForUpload.Image);

                    organization.LastModified = DateTime.Today;

                    await _unitOfWork.OrganizationRepository.Create(organization);
                    await _unitOfWork.SaveChangesAsync();

                    var organizationDtoForDisplay = _mapper.OrganizationToOrganizationDTOForDisplay(organization);

                    return Result<OrganizationDTOForDisplay>.SuccessResult(organizationDtoForDisplay);
                }
                return Result.FailureResult("Datos proporcionados erroneos", 400);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Result> Update(int id, OrganizationDTOForUpload organizationDTOForUpload)
        {
            try
            {
                var organization = await _unitOfWork.OrganizationRepository.GetByIdAsync(id);

                if (organization is not null)
                {
                    if (organization.SoftDelete)
                        return Result.FailureResult($"id({organization.Id}) esta eliminado del sistema.", 400);

                    var ValidationName = await _unitOfWork.OrganizationRepository.FindByConditionAsync(x => x.Name == organizationDTOForUpload.Name);
                    if (ValidationName.Count > 0)
                        return Result.FailureResult("Una organizacion con ese nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);

                    organization.Name = organizationDTOForUpload.Name;
                    organization.Address = organizationDTOForUpload.Address;
                    organization.Phone = organizationDTOForUpload.Phone;
                    organization.Email = organizationDTOForUpload.Email;
                    organization.WelcomeText = organizationDTOForUpload.WelcomeText;
                    organization.AboutUsText = organizationDTOForUpload.AboutUsText;
                    organization.FacebookUrl = organizationDTOForUpload.FacebookUrl;
                    organization.InstagramUrl = organizationDTOForUpload.InstagramUrl;
                    organization.LinkedinUrl = organizationDTOForUpload.LinkedinUrl;
                    organization.LastModified = DateTime.Now;

                    if (organizationDTOForUpload.Image is not null)
                    {
                        await _imageService.AwsDeleteFile(organization.Image[(organization.Image.LastIndexOf("/") + 1)..]);
                        organization.Image = await _imageService.UploadFile(organizationDTOForUpload.Image.FileName, organizationDTOForUpload.Image);
                    }

                    this._unitOfWork.OrganizationRepository.Update(organization);
                    await this._unitOfWork.SaveChangesAsync();

                    var organizationDTOForDisplayy = _mapper.OrganizationToOrganizationDTOForDisplay(organization);
                    return Result<OrganizationDTOForDisplay>.SuccessResult(organizationDTOForDisplayy);
                }

                return Result.FailureResult("id de organizacion inexistente.", 404);
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Error al actualizar la organizacion: " + ex.Message, 500);
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                var organization = await this._unitOfWork.OrganizationRepository.GetByIdAsync(id);

                if (organization is not null)
                {
                    if (organization.SoftDelete)
                        return Result.FailureResult($"id({organization.Id}) ya esta eliminado del sistema.", 400);

                    organization.SoftDelete = true;
                    organization.LastModified = DateTime.Today;

                    await this._unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult($"Organización:({organization.Id}) ha sido eliminada exitosamente.");
                }

                return Result.FailureResult("id de organizacion inexistente.", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }
    }
}
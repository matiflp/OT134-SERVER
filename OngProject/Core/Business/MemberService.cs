using OngProject.Core.Interfaces;
using OngProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Response;
using OngProject.Core.Helper;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Paged;
using Microsoft.AspNetCore.Http;

namespace OngProject.Core.Business
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _entityMapper;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContext;

        public MemberService(IUnitOfWork unitOfWork,
                             IEntityMapper mapper,
                             IHttpContextAccessor httpContext,
                             IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _entityMapper = mapper;
            _imageService = imageService;
            _httpContext = httpContext;
        }

        public async Task<Result> GetAll(PaginationParams paginationParams)
        {
            try
            {
                var members = await _unitOfWork.MembersRepository.FindAllAsync(null, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalMembers = await _unitOfWork.MembersRepository.Count();

                if (totalMembers == 0)
                    return Result.FailureResult("No existen Miembros que mostrar", 404);

                if (members.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados disponibles", 400);

                var Dto = members
                    .Where(member => !member.SoftDelete)
                    .Select(member => _entityMapper.MemberToMemberDTODisplay(member));

                var paged = PagedList<MemberDTODisplay>.Create(Dto.ToList(), totalMembers,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<MemberDTODisplay>(paged, url);

                return Result<PagedResponse<MemberDTODisplay>>.SuccessResult(pagedResponse);
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
                var member = await _unitOfWork.MembersRepository.GetByIdAsync(id);

                if (member is not null)
                {
                    if (member.SoftDelete)
                        return Result.FailureResult($"id({member.Id}) se encuentra eliminada del sistema.", 400);

                    var memberDtoDisplay = _entityMapper.MemberToMemberDTODisplay(member);
                    return Result<MemberDTODisplay>.SuccessResult(memberDtoDisplay);
                }

                return Result.FailureResult("El miembro no existe.", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Insert(MemberDTORegister memberDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(memberDTO.Name))              
                    return Result.FailureResult("Nombre requerido", 400);

                var resultName = await _unitOfWork.MembersRepository.FindByConditionAsync(x => x.Name == memberDTO.Name);
                if (resultName.Count > 0)
                    return Result.FailureResult("El nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);

                var member = _entityMapper.MemberDTORegisterToMember(memberDTO);

                member.LastModified = DateTime.Now;
                member.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{memberDTO.File.FileName}", memberDTO.File); ;

                await _unitOfWork.MembersRepository.Create(member);
                await _unitOfWork.SaveChangesAsync();

                var memberDisplay = _entityMapper.MemberToMemberDTODisplay(member);
                return Result<MemberDTODisplay>.SuccessResult(memberDisplay);
            }
            catch (Exception ex)
            {
                return Result.FailureResult($"Miembro no registrado: {ex.Message}", 500);
            }
        }

        public async Task<Result> Update(int id, MembersDtoForUpload memberDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(memberDTO.Name))
                    return Result.FailureResult("Nombre requerido", 400);

                var resultName = await _unitOfWork.MembersRepository.FindByConditionAsync(x => x.Name == memberDTO.Name);
                if (resultName.Count > 0)
                    return Result.FailureResult("El nombre ya existe en el sistema, intente uno diferente al ingresado.", 400);

                var member = await _unitOfWork.MembersRepository.GetByIdAsync(id);
                if (member is null) 
                    return Result.FailureResult("No se encontro el id", 404);

                if (member.SoftDelete)
                    return Result.FailureResult("El miembro se encuentra eliminado del sistema", 400);

                member.Name = memberDTO.Name;
                member.FacebookUrl = memberDTO.FacebookUrl;
                member.InstagramUrl = memberDTO.InstagramUrl;
                member.LinkedinUrl = memberDTO.LinkedinUrl;
                member.LastModified = DateTime.Now;

                if (memberDTO.Image is not null) 
                {
                    await _imageService.AwsDeleteFile(member.Image[(member.Image.LastIndexOf("/") + 1)..]);
                    member.Image = await _imageService.UploadFile($"{Guid.NewGuid()}_{memberDTO.Image.FileName}", memberDTO.Image);
                }

                await _unitOfWork.SaveChangesAsync();

                var memberDisplay = _entityMapper.MemberToMemberDTODisplay(member);
                return Result<MemberDTODisplay>.SuccessResult(memberDisplay);
            }
            catch (Exception ex)
            {
                return Result.FailureResult($"Error al actualizar al miembro: {ex.Message}", 500);
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                var member = await _unitOfWork.MembersRepository.GetByIdAsync(id);

                if (member is not null)
                {
                    if (member.SoftDelete)
                        return Result.FailureResult("El miembro ya se encuentra eliminado del sistema", 400);

                    member.SoftDelete = true;
                    member.LastModified = DateTime.Now;

                    await _unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult($"Miembro:({member.Id}) ha sido eliminado exitosamente.", 200);
                }

                return Result.FailureResult("No existe un miembro con ese Id", 404);
            }
            catch (Exception ex)
            {
                return Result.FailureResult($"Error al eliminar al miembro: {ex.Message}", 500);
            }
        }
    }
}
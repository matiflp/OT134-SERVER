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
using System.Security.Claims;
using System.Threading.Tasks;

namespace OngProject.Core.Business
{
    public class CommentsService : ICommentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public CommentsService(IUnitOfWork unitOfWork, IEntityMapper mapper, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<Result> GetAll(PaginationParams paginationParams)
        {
            try
            {
                var comments = await _unitOfWork.CommentsRepository.FindAllAsync(null, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.CommentsRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen comentarios", 404);

                if (comments.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados", 400);

                var commentsDTO = comments
                    .Where(newEntity => !newEntity.SoftDelete)
                    .OrderBy(comment => comment.LastModified)
                    .Select(comment => _mapper.CommentToCommentDtoForDisplay(comment));

                var paged = PagedList<CommentDtoForDisplay>.Create(commentsDTO.ToList(), totalCount,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<CommentDtoForDisplay>(paged, url);

                return Result<PagedResponse<CommentDtoForDisplay>>.SuccessResult(pagedResponse);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { e.Message }, 500);
            }
        }
        
        public async Task<Result> GetById(int idNew)
        {
            try
            {
                if (idNew == 0)
                    return Result.FailureResult("Debe seleccionar una novedad", 400);

                var allComments = await _unitOfWork.CommentsRepository.FindAllAsync();

                if (allComments is not null)
                {
                    // Se traen todos los comentarios de esa novedad que no esten eliminados
                    var listComments = allComments
                        .Where(comment => comment.NewId == idNew && !comment.SoftDelete)
                        .OrderBy(comment => comment.LastModified)
                        .Select(comment => _mapper.CommentToCommentDtoForDisplay(comment)) as ICollection<CommentDtoForDisplay>; ;

                    if (listComments is null || listComments.Count == 0)
                        return Result.FailureResult("No existen comentarios en la novedad con ese Id.", 404);

                    return Result<IEnumerable<CommentDtoForDisplay>>.SuccessResult(listComments);
                }

                return Result.FailureResult("No existen comentarios", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Insert(CommentDtoForRegister commentDTO)
        {
            try
            {
                var claim = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);             
                if (claim == null)
                    return Result.FailureResult("Debe estar registrado para agregar un comentario", 400);

                if (commentDTO.NewId == 0)
                    return Result.FailureResult("Se debe agregar una noticia", 400);
                
                if (string.IsNullOrEmpty(commentDTO.Body))
                    return Result.FailureResult("Se debe agregar un Comentario", 400);

                var comment = _mapper.CommentForRegisterToComment(commentDTO);

                comment.UserId = Int32.Parse(claim.Value);
                comment.LastModified = DateTime.Today;

                await _unitOfWork.CommentsRepository.Create(comment);
                await _unitOfWork.SaveChangesAsync();

                var commentDtoForDisplay = _mapper.CommentToCommentDtoForDisplay(comment);
                return Result<CommentDtoForDisplay>.SuccessResult(commentDtoForDisplay);
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Ocurrio un problema al momento de agregar un Comentario : " + ex.ToString(), 500);
            }
        }

        public async Task<Result> Update(int idComment, CommentDtoForRegister commentDto)
        {
            try
            {
                var claim = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (claim is null)
                    return Result.FailureResult("Debe estar registrado para borrar un comentario", 401);

                if (commentDto.NewId == 0)
                    return Result.FailureResult("Se debe agregar una noticia", 400);

                if (string.IsNullOrEmpty(commentDto.Body))
                    return Result.FailureResult("Se debe agregar un Comentario", 400);

                var comment = await _unitOfWork.CommentsRepository.GetByIdAsync(idComment);
                if (comment is null)
                    return Result.FailureResult("No se encontro comentario", 404);

                if (comment.SoftDelete)
                    return Result.FailureResult("No se encontro comentario", 400);

                var idUser = Int32.Parse(claim.Value);
                var rolUser = await _unitOfWork.UserRepository.GetByIdAsync(idUser);
                if (idUser != comment.UserId && rolUser.Rol.Name != "Administrator")
                    return Result.FailureResult("Usted no tiene permiso para modificar este comentario", 403);

                comment.Body = commentDto.Body;
                comment.LastModified = DateTime.Today;

                await _unitOfWork.SaveChangesAsync();

                var commentDisplay = _mapper.CommentToCommentDtoForDisplay(comment);
                return Result<CommentDtoForDisplay>.SuccessResult(commentDisplay);
            }
            catch(Exception ex)
            {
                return Result.FailureResult(ex.Message, 500);
            }
        }

        public async Task<Result> Delete(int idComment)
        {
            try
            {
                var claim = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (claim is null)                    
                    return Result.FailureResult("Debe estar registrado para borrar un comentario", 401);

                var comment = await _unitOfWork.CommentsRepository.GetByIdAsync(idComment);
                if (comment is null)
                    return Result.FailureResult("Comentario no encontrado", 404);

                if (comment.SoftDelete)
                    return Result.FailureResult("No se encontro comentario", 400);

                var idUser = Int32.Parse(claim.Value);
                var rolUser = await _unitOfWork.UserRepository.GetByIdAsync(idUser);
                if (idUser != comment.UserId && rolUser.Rol.Name != "Administrator")
                    return Result.FailureResult("Usted no tiene permiso para modificar este comentario", 403);

                comment.SoftDelete = true;
                comment.LastModified = DateTime.Now;

                await _unitOfWork.SaveChangesAsync();

                return Result<string>.SuccessResult($"Comentario {idComment} eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Ocurrio un Problema al eliminar el comentario : " + ex.ToString(), 500);
            }
        }
    }
}
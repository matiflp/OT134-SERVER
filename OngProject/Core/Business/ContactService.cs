using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using OngProject.Core.Helper;
using OngProject.Core.Interfaces;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using OngProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OngProject.Core.Models.Paged;

namespace OngProject.Core.Business
{
    public class ContactService : IContactService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public ContactService(IConfiguration configuration, IUnitOfWork unitOfWork, IEntityMapper mapper, IHttpContextAccessor httpContext)
        {
            _config = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<Result> GetAll(PaginationParams paginationParams)
        {
            try
            {
                var contact = await _unitOfWork.ContactRepository.FindAllAsync(null, null, null, paginationParams.PageNumber, paginationParams.PageSize);
                var totalCount = await _unitOfWork.ActivitiesRepository.Count();

                if (totalCount == 0)
                    return Result.FailureResult("No existen contactos", 404);

                if (contact.Count == 0)
                    return Result.FailureResult("paginacion invalida, no hay resultados", 400);

                var contactsDTO = contact
                    .Where(newEntity => !newEntity.SoftDelete)
                    .Select(activitie => _mapper.ContactToContactDTO(activitie));

                var paged = PagedList<ContactDTO>.Create(contactsDTO.ToList(), totalCount,
                                                                paginationParams.PageNumber,
                                                                paginationParams.PageSize);

                var url = $"{this._httpContext.HttpContext.Request.Scheme}://{this._httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.Path}";
                var pagedResponse = new PagedResponse<ContactDTO>(paged, url);

                return Result<PagedResponse<ContactDTO>>.SuccessResult(pagedResponse);
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
                var contact = await _unitOfWork.ContactRepository.GetByIdAsync(id);

                if (contact is not null)
                {
                    if (contact.SoftDelete)
                        return Result.FailureResult($"id({contact.Id}) se encuentra eliminada del sistema.", 400);

                    var contactDto = _mapper.ContactToContactDTO(contact);
                    return Result<ContactDTO>.SuccessResult(contactDto);
                }

                return Result.FailureResult("Ese contacto no existe.", 404);
            }
            catch (Exception ex)
            {
                return Result.ErrorResult(new List<string> { ex.Message }, 500);
            }
        }

        public async Task<Result> Insert(ContactDTO contactDto)
        {
            try
            {
                if (contactDto is not null)
                {
                    var contacts = _mapper.ContactDTOToContact(contactDto);

                    // Validaciones
                    if (string.IsNullOrEmpty(contacts.Email) && string.IsNullOrEmpty(contacts.Name))
                        return Result.FailureResult("Debes proporcionar tu nombre y una dirección de correo electronico", 400);

                    if (string.IsNullOrEmpty(contacts.Email))
                        return Result.FailureResult("Debes proporcionar un email", 400);

                    if (string.IsNullOrEmpty(contacts.Name))
                        return Result.FailureResult("Debes proporcionar tu nombre", 400);

                    contacts.LastModified = DateTime.Now;

                    await _unitOfWork.ContactRepository.Create(contacts);
                    await _unitOfWork.SaveChangesAsync();

                    //se envia mail de bienvenida
                    var emailSender = new EmailSender(_config);
                    var emailTitle = "Gracias por contactar con nosotros!";
                    var emailBody = $"<h4>Hola {contacts.Name}</h4><p> Hemos recibido su mensaje, en breve nos pondremos en contacto con usted.</p>";
                    var emailContact = string.Format("<a href='mailto:{0}'>{0}</a>", _config["MailParams:WelcomeMailContact"]);

                    await emailSender.SendEmailWithTemplateAsync(contacts.Email, emailTitle, emailBody, emailContact);

                    return Result<ContactDTO>.SuccessResult(_mapper.ContactToContactDTO(contacts));
                }

                return Result.FailureResult("Datos proporcionados erroneos", 400);
            }
            catch(Exception ex)
            {
                return Result.ErrorResult(new List<string> { "Error al guardar el contacto" + ex.Message }, 500);
            }
        }

        public async Task<Result> Update(int id, ContactDTO contactDto)
        {
            try
            {
                var contact = await _unitOfWork.ContactRepository.GetByIdAsync(id);

                if (contact is not null)
                {
                    if (contact.SoftDelete)
                        return Result.FailureResult("El contacto seleccionada esta eliminado", 400);

                    if (string.IsNullOrEmpty(contactDto.Email) && string.IsNullOrEmpty(contactDto.Name))
                        return Result.FailureResult("Debes proporcionar tu nombre y una dirección de correo electronico", 400);

                    if (string.IsNullOrEmpty(contactDto.Email))
                        return Result.FailureResult("Debes proporcionar un email", 400);

                    if (string.IsNullOrEmpty(contactDto.Name))
                        return Result.FailureResult("Debes proporcionar tu nombre", 400);

                    contact.Name = contactDto.Name;
                    contact.Phone = contactDto.Phone;
                    contact.Email = contactDto.Email;
                    contact.Message = contactDto.Message;
                    contact.LastModified = DateTime.Now;

                    await _unitOfWork.SaveChangesAsync();

                    var contactDtoDisplay = _mapper.ContactToContactDTO(contact);
                    return Result<ContactDTO>.SuccessResult(contactDtoDisplay);
                }

                return Result.FailureResult("No se encontro el id", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Error al actualizar el contacto: " + e.Message }, 500);
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                var contact = await _unitOfWork.ContactRepository.GetByIdAsync(id);

                if (contact is not null)
                {
                    if (contact.SoftDelete)
                        return Result.FailureResult("El contacto seleccionada ya fue eliminado", 400);

                    contact.SoftDelete = true;
                    contact.LastModified = DateTime.Now;

                    await _unitOfWork.SaveChangesAsync();

                    return Result<string>.SuccessResult($"Contacto {id} eliminado exitosamente.");
                }

                return Result.FailureResult("El contacto no existe.", 404);
            }
            catch (Exception e)
            {
                return Result.ErrorResult(new List<string> { "Error al eliminar el contacto: " + e.Message }, 500);
            }
        }

    }
}

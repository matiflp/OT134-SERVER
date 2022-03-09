using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OngProject.Core.Interfaces;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Paged;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OngProject.Controllers
{
    [SwaggerTag("Contacts", "Controller to create, read, update and delete contact entities.")]
    [Route("contacts")]
    [ApiController]
    [Authorize]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// GET: contacts
        /// <summary>
        ///     Gets all Contacts per page.
        /// </summary>
        /// <remarks>
        ///     Gets information paged about the Contacts in the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>        /// /// <response code="200">OK. Returns a result object along with the contacts information.</response>        
        /// <response code="200">OK. Returns contacts information.</response>      
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find contacts.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<ContactDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllContacts([FromQuery] PaginationParams pagingParams)
        {
            var result = await _contactService.GetAll(pagingParams);

            return StatusCode(result.StatusCode, result);
        }

        /// GET: contacts/id
        /// <summary>
        ///     Gets a contact information.
        /// </summary>
        /// <remarks>
        ///     Gets information about the contact with the id provided.
        /// </remarks>
        /// <param name="id">Contact id that will be searched.</param>        
        /// <response code="200">OK. Returns contact information.</response>  
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find contact.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<ActivityDTOForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _contactService.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: contacts
        /// <summary>
        ///     Creates a new contact.
        /// </summary>
        /// <remarks>
        ///     Adds a new contact in the database.
        /// </remarks>
        /// <param name="dto">New contact data transfer object.</param>
        /// <response code="200">OK. Returns a result object along with the new contact information.</response>        
        /// <response code="400">BadRequest. Contact could not be created.</response>       
        /// <response code="500">Internal Server Error.</response>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<ContactDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> Post([FromBody] ContactDTO contactDto)
        {
            var response = await _contactService.Insert(contactDto);

            return StatusCode(response.StatusCode, response);
        }

        /// PUT: contacts/id
        /// <summary>
        ///     Updates a contact.
        /// </summary>
        /// <remarks>
        ///     Updates a contact in the database.
        /// </remarks>
        /// <param name="contactDto">New value for the contact</param>
        /// <param name="id">Id from contact for changes</param>
        /// <response code="200">Ok. Return the new contact updated</response>
        /// <response code="400">BadRequest. Contact could not be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>   
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find contact.</response> 
        /// <response code="500">Internal Server Error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<ContactDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] ContactDTO contactDto)
        {
            var result = await _contactService.Update(id, contactDto);

            return StatusCode(result.StatusCode, result);
        }

        /// DELETE: contacts/id
        /// <summary>
        ///     Deletes a contact.
        /// </summary>
        /// <remarks>
        ///     Deletes a contact in the database.
        /// </remarks>
        /// <param name="id">Id of the contact that will be removed from the database</param>
        /// <response code="200">OK. Returns a result object if the contact was successfully removed.</response>        
        /// <response code="400">BadRequest. Contact could not be removed.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the contact with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id )
        {
            var result = await _contactService.Delete(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}

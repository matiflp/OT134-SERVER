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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OngProject.Controllers
{
    [SwaggerTag("Organizations", "Controller to create, read, update and delete organization entities.")]
    [Route("organizations")]
    [ApiController]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationsService _organizationsService;
        public OrganizationsController(IOrganizationsService organizationsService)
        {
            _organizationsService = organizationsService;
        }

        /// GET: organizations
        /// <summary>
        ///     Gets all organizations per page.
        /// </summary>
        /// <remarks>
        ///     Gets information paged about the organizations in the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>    
        /// <response code="200">OK. Returns organizations information.</response>  
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="404">Not found. Server couldn't find the organizations.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [Route("public")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<OrganizationDTOForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]       
        public async Task<IActionResult> Get([FromQuery] PaginationParams pagingParams)
        {
            var result = await _organizationsService.GetAll(pagingParams);

            return StatusCode(result.StatusCode, result);
        }

        /// GET: organizations/id
        /// <summary>
        ///     Gets an organization information.
        /// </summary>
        /// <remarks>
        ///     Gets information about the organization with the id provided.
        /// </remarks>
        /// <param name="id">Organization id that will be searched.</param>        
        /// <response code="200">OK. Returns organization information.</response> 
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="404">Not found. Server couldn't find the organization.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<OrganizationDTOForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _organizationsService.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: organizations
        /// <summary>
        ///     Creates a new organization.
        /// </summary>
        /// <remarks>
        ///     Adds a new organization in the database.
        /// </remarks>
        /// <param name="organizationDTOForUpload">New organization data transfer object.</param>
        /// <response code="200">OK. Returns a result object along with the new organization information.</response>        
        /// <response code="400">BadRequest. Organization could not be created.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="500">Internal Server Error.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<OrganizationDTOForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] OrganizationDTOForUpload organizationDTOForUpload)
        {
            var result = await _organizationsService.Insert(organizationDTOForUpload);

            return StatusCode(result.StatusCode, result);
        }

        /// PUT: organizations/id
        /// <summary>
        ///     Updates an organization.
        /// </summary>
        /// <remarks>
        ///     Updates an organization in the database.
        /// </remarks>
        /// <param name="organizationDTOForUpload">New value for the organization</param>
        /// <param name="id">Id from organization for changes</param>
        /// <response code="200">Ok. Return the new organization updated</response>
        /// <response code="400">BadRequest. Organization could not be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>   
        /// <response code="404">Not found. Server couldn't find the organization.</response> 
        /// <response code="500">Internal Server Error</response>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<OrganizationDTOForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id,[FromForm] OrganizationDTOForUpload organizationDTOForUpload)
        {
            var result = await _organizationsService.Update(id, organizationDTOForUpload);

            return StatusCode(result.StatusCode, result);
        }

        /// DELETE: organizations/id
        /// <summary>
        ///     Deletes an organization.
        /// </summary>
        /// <remarks>
        ///     Deletes an organization in the database.
        /// </remarks>
        /// <param name="id">Id of the organization that will be removed from the database</param>
        /// <response code="200">OK. Returns a result object if the organization was successfully removed.</response>        
        /// <response code="400">BadRequest. Organization could not be removed.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="404">Not found. Server couldn't find the organization with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _organizationsService.Delete(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
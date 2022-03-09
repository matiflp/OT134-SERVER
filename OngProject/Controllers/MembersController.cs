using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OngProject.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;
using OngProject.Core.Models.Paged;
using OngProject.Core.Models.Response;
using System.Collections.Generic;

namespace OngProject.Controllers
{
    [SwaggerTag("Members", "Controller to create, read, update and delete members entities.")]
    [Route("members")]
    [ApiController]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _membersService;
        public MembersController(IMemberService memberService)
        {
            _membersService = memberService;
        }

        /// GET: members
        /// <summary>
        ///    Gets all Members information.
        /// </summary>
        /// <remarks>
        ///     Gets information about all the current members stored in the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>            
        /// <response code="200">OK. Returns members information.</response>  
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>     
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not Found. Server couldn't find any members information.</response> 
        /// <response code="500">Internal Server Error.</response>  
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(Result<PagedResponse<MemberDTODisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllMembers([FromQuery] PaginationParams pagingParams)
        {
            var result = await _membersService.GetAll(pagingParams);

            return StatusCode(result.StatusCode, result);
        }

        /// GET: members/id
        /// <summary>
        ///     Gets a member information.
        /// </summary>
        /// <remarks>
        ///     Gets information about the member with the id provided.
        /// </remarks>
        /// <param name="id">Member id that will be searched.</param>        
        /// <response code="200">OK. Returns member information.</response>  
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find member.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<MemberDTODisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _membersService.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: members
        /// <summary>
        ///     Creates a new Member.
        /// </summary>
        /// <remarks>
        ///     Adds a new member to the database.
        /// </remarks>
        /// <param name="memberDTO">New Member object.</param>
        /// <response code="200">OK. Returns a result object along with the new member information.</response>        
        /// <response code="400">BadRequest. Member could not be created.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="500">Internal Server Error.</response>  
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(Result<MemberDTODisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] MemberDTORegister memberDTO)
        {
            var result = await _membersService.Insert(memberDTO);

            return StatusCode(result.StatusCode, result);
        }

        /// PUT: members/id
        /// <summary>
        ///     Updates a Member information.
        /// </summary>
        /// <remarks>
        ///     Updates a member information with the values provided.
        /// </remarks>
        /// <param name="id">Member id that will be updated.</param>
        /// <param name="memberDTO">Current Member information.</param>
        /// <response code="200">OK. Returns a result object along with the new member information.</response>   
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not Found. Server couldn't find the member with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(Result<MemberDTODisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromForm] MembersDtoForUpload memberDTO)
        {
            var result = await _membersService.Update(id, memberDTO);

            return StatusCode(result.StatusCode, result);
        }

        /// DELETE: members/id
        /// <summary>
        ///     Deletes a Member.
        /// </summary>
        /// <remarks>
        ///     Deletes a member from the database.
        /// </remarks>
        /// <param name="id">Member id that will be deleted.</param>
        /// <response code="200">OK. Returns a success message.</response>    
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not Found. Server couldn't find the member with id provided.</response> 
        /// <response code="500">Internal Server Error.</response>  
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _membersService.Delete(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
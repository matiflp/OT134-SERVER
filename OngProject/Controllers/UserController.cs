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
    [SwaggerTag("Users", "Controller to create, read, update and delete users entities.")]
    [Route("users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// GET: users
        /// <summary>
        ///     Get all users information.
        /// </summary>
        /// <remarks>
        ///     Get the information about all the users stored in the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param> 
        /// <response code="200">OK. Returns users information.</response>  
        /// <response code="400">Bad request. Invalid request received.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>     
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not Found. Server couldn't find any user.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<UserDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] PaginationParams pagingParams)
        {
            var result = await _userService.GetAll(pagingParams);

            return StatusCode(result.StatusCode, result);            
        }

        /// PUT: users/id
        /// <summary>
        ///     Updates an user information.
        /// </summary>
        /// <remarks>
        ///     Updates an user information with the values provided.
        /// </remarks>
        /// <param name="id">User ID that will be updated.</param>
        /// <param name="user">Dto that allow to update the user</param>
        /// <response code="200">OK. Returns a result object.</response>  
        /// <response code="400">Bad request. User couldn't be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>     
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not Found. Server couldn't find any user with the ID provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpPut("id")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<UserDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromForm] UserUpdateDto user)
        {
            //var claimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var response = await _userService.Update(id, user ); //Int32.Parse(claimId.Value)
            return StatusCode(response.StatusCode, response);
        }

        /// DELETE: users/id
        /// <summary>
        ///     Deletes an user.
        /// </summary>
        /// <remarks>
        ///     Deletes the user in the database.
        /// </remarks>
        /// <param name="id">User ID that will be deleted.</param>
        /// <response code="200">OK. Returns a result object.</response>  
        /// <response code="400">Bad request. User couldn't be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>     
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not Found. Server couldn't find any user with the ID provided.</response> 
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
            var result = await this._userService.Delete(id);
                     
            return StatusCode(result.StatusCode, result);
        }
    }
}


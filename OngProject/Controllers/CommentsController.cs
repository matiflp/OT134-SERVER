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
    [SwaggerTag("Categories", "Controller to create, read, update and delete comment entities.")]
    [Route("comments")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        /// GET: comments
        /// <summary>
        ///     Gets all comments per page.
        /// </summary>
        /// <remarks>
        ///     Gets information paged about the comments in the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>    
        /// <response code="200">OK. Returns comments information.</response>  
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find comments.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<CommentDtoForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] PaginationParams pagingParams)
        {
            var result = await _commentsService.GetAll(pagingParams);

            return StatusCode(result.StatusCode, result);
        }

        /// GET: comments/id
        /// <summary>
        ///     Gets a comments information.
        /// </summary>
        /// <remarks>
        ///     Gets information about comments belongs to the new id provided.
        /// </remarks>
        /// <param name="id">New id used to search for comments.</param>        
        /// <response code="200">OK. Returns a list of comments that belongs to a new.</response> 
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="404">Not found. Server couldn't find comments.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [AllowAnonymous]
        [Route("/news/{id}/comments")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<CommentDtoForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]       
        public async Task<IActionResult> Get(int id)
        {
            var result = await _commentsService.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: comments
        /// <summary>
        ///     Creates a new comment.
        /// </summary>
        /// <remarks>
        ///     Adds a new comment in the database.
        /// </remarks>
        /// <param name="dto">New comment data transfer object.</param>
        /// <response code="200">OK. Returns a result object along with the new comment information.</response>        
        /// <response code="400">BadRequest. Comment could not be created.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="500">Internal Server Error.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<CommentDtoForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CommentDtoForRegister dto)
        {
            var result = await _commentsService.Insert(dto);

            return StatusCode(result.StatusCode, result);
        }

        /// PUT: comments/id
        /// <summary>
        ///     Updates a comment.
        /// </summary>
        /// <remarks>
        ///     Updates a comment in the database.
        /// </remarks>
        /// <param name="commentDto">New value for the comment</param>
        /// <param name="id">Id from comment for changes</param>
        /// <response code="200">Ok. Return the new comment updated</response>
        /// <response code="400">BadRequest. Comment could not be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>   
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the comment.</response> 
        /// <response code="500">Internal Server Error</response>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<CommentDtoForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] CommentDtoForRegister commentDto)
        {
            var result = await _commentsService.Update(id, commentDto);

            return StatusCode(result.StatusCode, result);
        }

        /// DELETE: comments/id
        /// <summary>
        ///     Deletes a comment.
        /// </summary>
        /// <remarks>
        ///     Deletes an comment in the database.
        /// </remarks>
        /// <param name="id">Id of the comment that will be removed from the database</param>
        /// <response code="200">OK. Returns a result object if the comment was successfully removed.</response>        
        /// <response code="400">BadRequest. Comment could not be removed.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the activity with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {          
            var result = await _commentsService.Delete(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
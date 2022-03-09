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
    [SwaggerTag("Slides", "Controller to create, read, update and delete activity entities.")]
    [Route("slides")]
    [ApiController]
    [Authorize]
    public class SlideController : ControllerBase
    {
        private readonly ISlideSerivice _slideSerivice;
        public SlideController(ISlideSerivice slideSerivice)
        {
            _slideSerivice = slideSerivice;
        }

        /// GET: slides
        /// <summary>
        ///    Gets slides information.
        /// </summary>
        /// <remarks>
        ///     Gets information paged about the slides int the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>
        /// <response code="200">OK. Returns slides information.</response>  
        /// <response code="400">Bad request. Invalid request received.</response>     
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the slides.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<SlideDtoForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSlides([FromQuery] PaginationParams pagingParams)
        {
            var result = await _slideSerivice.GetAll(pagingParams);

            return StatusCode(result.StatusCode, result);
        }

        /// GET: slides/id
        /// <summary>
        ///    Gets a slide information.
        /// </summary>
        /// <remarks>
        ///     Gets information about the slide with the id provided.
        /// </remarks>
        /// <param name="id">Slide id that will be searched.</param>
        /// <response code="200">OK. Returns the slide information.</response>  
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the slide with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<SlideDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSlide(int id)
        {
            var result = await _slideSerivice.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: slides
        /// <summary>
        ///     Creates a new slide.
        /// </summary>
        /// <remarks>
        ///     Adds a new slide in the database.
        /// </remarks>
        /// <param name="slideDto">New slide data transfer object.</param>
        /// <response code="200">OK. Returns a result object along with the new slide information.</response>        
        /// <response code="400">BadRequest. Slide could not be created.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<SlideDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSlide([FromForm] SlideDtoForUpload slideDto)
        {
            var result = await _slideSerivice.Insert(slideDto);

            return StatusCode(result.StatusCode, result);
        }

        /// PUT: slides/id
        /// <summary>
        ///     Updates a slide.
        /// </summary>
        /// <remarks>
        ///     Updates a slide in the database.
        /// </remarks>
        /// <param name="id">Slide id that will be updated.</param>
        /// <param name="dto">Slide data transfer object.</param>
        /// <response code="200">OK. Returns a result object if the slide was successfully updated.</response>        
        /// <response code="400">BadRequest. Slide could not be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the slide with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<SlideDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromForm] SlideDtoForUpdate dto)
        {
            var result = await _slideSerivice.Update(id, dto);

            return StatusCode(result.StatusCode, result);
        }

        /// DELETE: slides/id
        /// <summary>
        ///     Deletes a slide.
        /// </summary>
        /// <remarks>
        ///     Deletes a slide in the database.
        /// </remarks>
        /// <param name="id">Id of the slide that will be removed in the database.</param>
        /// <response code="200">OK. Returns a result object if the slide was successfully removed.</response>        
        /// <response code="400">BadRequest. Slide could not be removed.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the slide with the id provided.</response> 
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
        public async Task<IActionResult> DeleteSlide(int id)
        {
            var result = await _slideSerivice.Delete(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
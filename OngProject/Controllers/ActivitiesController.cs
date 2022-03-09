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
    [SwaggerTag("Activities", "Controller to create, read, update and delete activity entities.")]
    [Route("activities")]
    [ApiController]
    [Authorize]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivitiesService _activitiesService;
        public ActivitiesController(IActivitiesService activitiesService)
        {
            _activitiesService = activitiesService;
        }

        /// GET: activities
        /// <summary>
        ///     Gets all activities per page.
        /// </summary>
        /// <remarks>
        ///     Gets information paged about the activities in the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>    
        /// <response code="200">OK. Returns activities information.</response>  
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="404">Not found. Server couldn't find the activities.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<ActivityDTOForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] PaginationParams pagingParams)
        {
            var result = await _activitiesService.GetAll(pagingParams);
            
            return StatusCode(result.StatusCode, result);
        }

        /// GET: activities/id
        /// <summary>
        ///     Gets an activity information.
        /// </summary>
        /// <remarks>
        ///     Gets information about the activity with the id provided.
        /// </remarks>
        /// <param name="id">Activity id that will be searched.</param>        
        /// <response code="200">OK. Returns activity information.</response> 
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="404">Not found. Server couldn't find the activity.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<ActivityDTOForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> Get(int id)
        {
            var result = await _activitiesService.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: activities
        /// <summary>
        ///     Creates a new Activity.
        /// </summary>
        /// <remarks>
        ///     Adds a new activity in the database.
        /// </remarks>
        /// <param name="dto">New Activity data transfer object.</param>
        /// <response code="200">OK. Returns a result object along with the new activity information.</response>        
        /// <response code="400">BadRequest. Activity could not be created.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="500">Internal Server Error.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<ActivityDTOForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] ActivityDTOForRegister dto)
        {
            var result = await _activitiesService.Insert(dto);

            return StatusCode(result.StatusCode, result);          
        }

        /// PUT: activities/id
        /// <summary>
        ///     Updates an activity.
        /// </summary>
        /// <remarks>
        ///     Updates an activity in the database.
        /// </remarks>
        /// <param name="activitiesDto">New value for the activity</param>
        /// <param name="id">Id from activity for changes</param>
        /// <response code="200">Ok. Return the new activity updated</response>
        /// <response code="400">BadRequest. Activity could not be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>   
        /// <response code="404">Not found. Server couldn't find the activity.</response> 
        /// <response code="500">Internal Server Error</response>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<ActivityDTOForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromForm] ActivitiesDtoForUpload activitiesDto)
        {
            var result = await _activitiesService.Update(id, activitiesDto);

            return StatusCode(result.StatusCode, result);
        }

        /// DELETE: activities/id
        /// <summary>
        ///     Deletes an activity.
        /// </summary>
        /// <remarks>
        ///     Deletes an Activity in the database.
        /// </remarks>
        /// <param name="id">Id of the activity that will be removed from the database</param>
        /// <response code="200">OK. Returns a result object if the activity was successfully removed.</response>        
        /// <response code="400">BadRequest. Activity could not be removed.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="404">Not found. Server couldn't find the activity with the id provided.</response> 
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
            var result = await _activitiesService.Delete(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OngProject.Core.Interfaces;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.PagedResourceParameters;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;
using OngProject.Core.Models.Response;
using OngProject.Core.Models.Paged;
using System.Collections.Generic;

namespace OngProject.Controllers
{
    [SwaggerTag("Testimonials", "Controller to get all the testimonials by page, to post one and delete it.")]
    [Route("testimonials")]
    [ApiController]
    [Authorize]
    public class TestimonialsController : ControllerBase
    {
        private readonly ITestimonialsService _testimonialsService;
        public TestimonialsController(ITestimonialsService testimonialsService)
        {
            _testimonialsService = testimonialsService;
        }

        /// GET: testimonials
        /// <summary>
        ///     User testimonials.
        /// </summary>
        /// <remarks>
        ///     User testimonials about how the ONG helped them.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>           
        /// <response code="200">OK. Return an object Result that includes the Testimonial.</response>        
        /// <response code="400">BadRequest. Return an object Result with an error message that indicate the cause of the problem.</response>  
        /// <response code="401">Authorization Required. Returns a Result object with a message indicating that the cause of the problem is that the user did not register and/or log in to the system.</response>  
        /// <response code="404">Not found.The server does not contain Testimonials.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<TestimonialDTODisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagingParams)
        {
            var result = await _testimonialsService.GetAll(pagingParams);

            return StatusCode(result.StatusCode, result);
        }

        /// GET: testimonials/id
        /// <summary>
        ///     Gets an testimonial information.
        /// </summary>
        /// <remarks>
        ///     Gets information about the testimonial with the id provided.
        /// </remarks>
        /// <param name="id">Testimonial id that will be searched.</param>        
        /// <response code="200">OK. Returns testimonial information.</response> 
        /// <response code="400">BadRequest. Invalid request received.</response> 
        /// <response code="401">Authorization Required. Returns a Result object with a message indicating that the cause of the problem is that the user did not register and/or log in to the system.</response>  
        /// <response code="404">Not found. Server couldn't find the testimonial.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<TestimonialDTODisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _testimonialsService.GetById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: testimonials
        /// <summary>
        ///     To add a new Testimonial.
        /// </summary>
        /// <remarks>
        ///     New Testimonial from a user.
        /// </remarks>
        /// <param name="testimonialDTO">Testimonial to save in the database.</param>  
        /// <response code="200">OK. Return an object Result that include the testimonial just added.</response>        
        /// <response code="400">BadRequest. Return an object Result with an error message that indicate the cause of the problem.</response>  
        /// <response code="401">Authorization Required. Returns a Result object with a message indicating that the cause of the problem is that the user did not register and/or log in to the system.</response>  
        /// <response code="500">Internal Server Error.</response>        
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<TestimonialDTODisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] TestimonialDTO testimonialDTO)
        {
            var response = await _testimonialsService.Insert(testimonialDTO);

            return StatusCode(response.StatusCode, response);
        }

        /// PUT: testimonials/id
        /// <summary>
        ///     Updates a testimonial.
        /// </summary>
        /// <remarks>
        ///     Updates a testimonial from the database.
        /// </remarks>
        /// <param name="id">Id of the object to update.</param>         
        /// <response code="200">OK. Return an object Result indicating that the testimonial was updated in the Db.</response>        
        /// <response code="400">BadRequest. Return an object Result with an error message that indicate the cause of the problem.</response>  
        /// <response code="401">Authorization Required. Returns a Result object with a message indicating that the cause of the problem is that the user did not register and/or log in to the system.</response>  
        /// <response code="404">NotFound. returns a message warning that the data does not exist.</response>
        /// <response code="500">Internal Server Error.</response>              
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<TestimonialDTODisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromForm] TestimonialDTO TestimonialDto)
        {
            var response = await _testimonialsService.Update(id, TestimonialDto);

            return StatusCode(response.StatusCode, response);
        }

        /// DELETE: testimonials/id
        /// <summary>
        ///     Deletes a Testimonial.
        /// </summary>
        /// <remarks>
        ///     Deletes a Testimonial from the database.
        /// </remarks>
        /// <param name="id">Id of the object to delete.</param>         
        /// <response code="500">Internal Server Error.</response>              
        /// <response code="200">OK. returns a message showing that the testimonial was deleted.</response>        
        /// <response code="400">BadRequest. Return an object Result with an error message that indicate the cause of the problem.</response>  
        /// <response code="401">Authorization Required. Returns a Result object with a message indicating that the cause of the problem is that the user did not register and/or log in to the system.</response>  
        /// <response code="404">Not found.The server does not contain Testimonial.</response> 
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _testimonialsService.Delete(id);

            return StatusCode(response.StatusCode, response);
        }
    }
}

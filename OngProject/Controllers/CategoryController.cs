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
    [SwaggerTag("Categories", "Controller to create, read, update and delete category entities.")]
    [Route("categories")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// GET: categories
        /// <summary>
        ///    Gets categories information.
        /// </summary>
        /// <remarks>
        ///     Gets information paged about the categories in the database.
        /// </remarks>
        /// <param name="pagingParams"> Page number(optional), if present it must be a number greather than 0 and
        ///  Page Size (optional), number of results per page, if present it must be be a number beetween 1 and 50.</param>
        /// <response code="200">OK. Returns categories information.</response>  
        /// <response code="400">Bad request. Invalid request received.</response>     
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the categories.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<PagedResponse<CategoryDtoForDisplay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginationParams pagingParams)
        {            
            var result = await _categoryService.GetAll(pagingParams);
            
            return StatusCode(result.StatusCode, result);
        }

        /// GET: categories/id
        /// <summary>
        ///    Gets a Category information.
        /// </summary>
        /// <remarks>
        ///     Gets information about the category with the id provided.
        /// </remarks>
        /// <param name="id">Category id that will be searched.</param>
        /// <response code="200">OK. Returns the category information.</response>  
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="403">Forbidden. You do not have the permissions to perform this action.</response>
        /// <response code="404">Not found. Server couldn't find the category with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<CategoryDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryById(int id)
        {            
            var result = await _categoryService.GetById(id);

            return StatusCode(result.StatusCode, result);           
        }

        /// POST: categories
        /// <summary>
        ///     Creates a new Category.
        /// </summary>
        /// <remarks>
        ///     Adds a new Category in the database.
        /// </remarks>
        /// <param name="categoryDTO">New Category data transfer object.</param>
        /// <response code="200">OK. Returns a result object along with the new category information.</response>        
        /// <response code="400">BadRequest. Category could not be created.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>    
        /// <response code="500">Internal Server Error.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<CategoryDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]       
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> Insert([FromForm] CategoryDTOForRegister categoryDTO)
        {
            var response = await _categoryService.Insert(categoryDTO);
            
            return StatusCode(response.StatusCode, response);
        }

        /// PUT: categories/id
        /// <summary>
        ///     Updates a Category.
        /// </summary>
        /// <remarks>
        ///     Updates a Category in the database.
        /// </remarks>
        /// <param name="id">Category id that will be removed.</param>
        /// <param name="categoryDTO">Category data transfer object.</param>
        /// <response code="200">OK. Returns a result object if the category was successfully updated.</response>   
        /// <response code="400">BadRequest. Category could not be updated.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="404">Not found. Server couldn't find the category with the id provided.</response> 
        /// <response code="500">Internal Server Error.</response>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<CategoryDtoForDisplay>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status401Unauthorized)]      
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromForm] CategoryDTOForUpload dto)
        {
            var result = await _categoryService.Update(id, dto);

            return StatusCode(result.StatusCode, result);
        }

        /// DELETE: categories/id
        /// <summary>
        ///     Deletes a Category.
        /// </summary>
        /// <remarks>
        ///     Deletes a Category in the database.
        /// </remarks>
        /// <param name="id">Id of the Category that will be removed in the database.</param>
        /// <response code="200">OK. Returns a result object if the category was successfully removed.</response>        
        /// <response code="400">BadRequest. Category could not be removed.</response>    
        /// <response code="401">Unauthorized. Invalid JWT Token or it wasn't provided.</response>
        /// <response code="404">Not found. Server couldn't find the category with the id provided.</response> 
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
            var result = await _categoryService.Delete(id);
            
            return StatusCode(result.StatusCode, result);
        }
    }

}

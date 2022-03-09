using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using OngProject.Core.Models.Response;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;

namespace OngProject.Controllers
{
    [SwaggerTag("Auth", "Controller to register, login and get account details")]
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            this._userService = userService;
        }

        /// POST: auth/login
        /// <summary>
        ///     Login to enter the system.
        /// </summary>
        /// <remarks>
        ///     When you log in you will have the possibility of accessing new functionalities.
        /// </remarks>
        /// <param name="userLoginDto">Email and password of the user.</param>
        /// <response code="200">OK. Return an object Result returns a result object along with a generated token to login.</response>        
        /// <response code="400">BadRequest. User could not enter.</response>  
        /// <response code="500">Internal Server Error.</response>              
        [HttpPost]
        [Route("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]      
        public async Task<IActionResult> Login([FromBody]  UserLoginDTO userLoginDto)
        {
            var result = await _userService.LoginAsync(userLoginDto);

            return StatusCode(result.StatusCode, result);
        }

        /// POST: auth/register
        /// <summary>
        ///     Creates a new User.
        /// </summary>
        /// <remarks>
        ///     Adds a new user who will have the possibility of accessing new functionalities.
        /// </remarks>
        /// <param name="dto">New User.</param>
        /// <response code="200">OK. Return an object Result returns a result object along with a generated token to login.</response>        
        /// <response code="400">BadRequest. User could not be created.</response> 
        /// <response code="500">Internal Server Error.</response>              
        [HttpPost]
        [Route("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromForm] UserRegisterDto dto)
        {
            var result = await _userService.Insert(dto);

            return StatusCode(result.StatusCode, result);
        }

        /// GET: auth/me
        /// <summary>
        ///     User account detail.
        /// </summary>
        /// <remarks>
        ///     User information stored in the databaseies.
        /// </remarks>
        /// <response code="200">OK. Returns user account detail.</response>        
        /// <response code="404">Not found. Server couldn't find the user.</response> 
        /// <response code="500">Internal Server Error.</response> 
        [HttpGet]
        [Route("me")]
        [Authorize]
        [Produces("application/json")]     
        [ProducesResponseType(typeof(Result<UserDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Me()
        {
            try
            {
                var claimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (claimId is null)
                    return (IActionResult)Result.FailureResult("id de usuario inexistente.", 404);

                var result = await this._userService.GetById(Int32.Parse(claimId.Value));

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, Result.ErrorResult(new List<string> { e.Message }));
            }

        }
    }
}

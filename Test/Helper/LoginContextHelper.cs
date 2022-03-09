using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OngProject.Controllers;
using OngProject.Core.Business;
using OngProject.Core.Helper;
using OngProject.Core.Interfaces;
using OngProject.Core.Models.DTOs;
using OngProject.Repositories.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Test.Helper
{
    public class LoginContextHelper
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginContextHelper(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ControllerContext> GetLoginContext(string email, string password)
        {
            var loginDTO = new UserLoginDTO
            {
                Email = email,
                Password = password
            };

            var loginController = new AuthController(_userService);

            var loginResponse = await loginController.Login(loginDTO);
            var r = loginResponse as ObjectResult;
            string token = r.Value.ToString();

            var userResult = await _unitOfWork.UserRepository.FindByConditionAsync(x => x.Email == email);
            var user = userResult.FirstOrDefault();

            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                        new Claim(ClaimTypes.Name, user.Email),
                                        new Claim(ClaimTypes.Role, user.Rol?.Name)
                                   }, "TestAuthentication"));

            var httpContext = new DefaultHttpContext() { User = userClaim };
            httpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

            return new ControllerContext()
            {
                HttpContext = httpContext,
            };
        }
    }
}

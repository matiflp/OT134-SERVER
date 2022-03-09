using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OngProject.Controllers;
using OngProject.Core.Business;
using OngProject.Core.Models.DTOs;
using OngProject.Repositories;
using OngProject.Core.Helper;
using System.Threading.Tasks;
using Test.Helper;
using OngProject.Core.Interfaces;
using OngProject.Repositories.Interfaces;
using OngProject.DataAccess;
using Microsoft.AspNetCore.Http;
using OngProject.Core.Models.Response;
using System.Security.Claims;
using OngProject.Core.Mapper;

namespace Test
{
    [TestClass]
    public class AuthControllerTest
    {
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configurations;
        private JwtHelper jwtHelper;
        private IUserService _userService;
        private IImageService _imageService;
        private IEntityMapper _entityMapper;
        private IHttpContextAccessor _httpContext;

        [TestInitialize]
        public void Init()
        {
            _configurations = ConfigurationHelper.SetConfigurations();
            
            _context = DbContextHelper.MakeDbContext(true);
            _unitOfWork = new UnitOfWork(_context);
            
            _imageService = new ImageService(_unitOfWork);
            jwtHelper = new JwtHelper(_configurations);
            _entityMapper = new EntityMapper();
            _httpContext = new HttpContextAccessor();

            _userService = new UsersService(_unitOfWork, _configurations, jwtHelper, _imageService, _entityMapper, _httpContext);
        }

        [TestMethod]
        [DataRow("User1@ong.com", "Password1", 200)] // Login succesfull.
        [DataRow("T@gmail.com", "Password1", 400)] // Email doesn't exists.
        [DataRow("User1@ong.com", "123456789", 400)] // Wrong password.
        public async Task LoginTestInitSessionEmailAndPassword(string email, string password, int expected)
        {
            using (_context)
            {
                //Arrange
                var loginDTO = new UserLoginDTO
                {
                    Email = email,
                    Password = password
                };
                var loginController = new AuthController(_userService);

                //Act
                var login = await loginController.Login(loginDTO);
                var actual = login as ObjectResult;

                //Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual.StatusCode);
            }
        }

        [TestMethod]
        [DataRow("mflopezartes@gmail.com", "123456", 200)] // Register succesfull and email sent.
        [DataRow("User1@ong.com", "123456", 400)] // Email in use.
        public async Task RegisterTestSignUpSuccessEmailInUse(string email, string password, int expected)
        {
            using (_context)
            {
                //Arrange
               var registerDto = new UserRegisterDto
               {
                   Email = email,
                   Password = password,
                   FirstName = "Test First Name",
                   LastName = "Teste Last Name",
                   Photo = ImageHelper.GetImage(),
                   RolId = 1
               };
               var registerController = new AuthController(_userService);

               //Act
               var register = await registerController.Register(registerDto);
               var actual = register as ObjectResult;

               //Assert
               Assert.IsNotNull(actual);
               Assert.AreEqual(expected, actual.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetDataUser()
        {
            using (_context)
            {
                //Arrange
                var loginDTO = new UserLoginDTO
                {
                    Email = "User2@ong.com",
                    Password = "Password2"
                };
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, "2"),
                    new Claim(ClaimTypes.Name, "User2@ong.com"),
                    new Claim(ClaimTypes.Role, "User")
                }, "TestAuthentication"));
                var authController = new AuthController(_userService);

                //Act
                var login = await authController.Login(loginDTO);
                var r = login as ObjectResult;
                
                Result<string> result = r.Value as Result<string>;
                var token = result.Data;

                var httpContext = new DefaultHttpContext() 
                {
                    User = user
                };
                httpContext.Request.Headers.Add("Authorization", "Bearer " + token);
                var controllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                };

                authController.ControllerContext = controllerContext;

                var response = await authController.Me();
                var expected = response as ObjectResult;

                //Assert                
                Assert.AreEqual(200, expected.StatusCode);
            }
        }
    }
}
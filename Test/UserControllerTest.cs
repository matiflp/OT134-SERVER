using OngProject.Controllers;
using Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OngProject.Core.Business;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OngProject.Core.Models.Response;
using OngProject.Core.Models.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OngProject.DataAccess;
using OngProject.Core.Helper;
using OngProject.Core.Mapper;
using OngProject.Repositories;
using OngProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OngProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using OngProject.Core.Models.PagedResourceParameters;
using Moq;
using OngProject.Core.Models.Paged;

namespace Test
{
    [TestClass]
    public class UserControllerTest
    {
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configurations;
        private JwtHelper _jwtHelper;
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
            _jwtHelper = new JwtHelper(_configurations);
            _entityMapper = new EntityMapper();
            _httpContext = new HttpContextAccessor();

            _userService = new UsersService(_unitOfWork, _configurations, _jwtHelper, _imageService, _entityMapper, _httpContext);
        }

        [TestMethod]
        public async Task DeleteUserSuccessTest()
        {
            // Arrange
            var userController = new UserController(_userService);            
            var user = await _unitOfWork.UserRepository.GetByIdAsync(2);
            
            // Act
            var response = await userController.Delete(user.Id);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(user.SoftDelete);            
        }        

        [TestMethod]
        public async Task DeleteUserFailUserNotExistTest()
        {
            // Arrange
            var userController = new UserController(_userService);            

            // Act
            var response = await userController.Delete(50);            
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteUserFailUserAlreadyDeletedTest()
        {
            // Arrange
            var userController = new UserController(_userService);

            // Act
            var response = await userController.Delete(1); // seed de usuario 1 se crea eliminado
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public async Task GetAllUsersSuccessTest()
        {
            // Arrange
            HostString myUri = new HostString("www.api.com:8080");
            var MockMemberHttpContex = new Mock<HttpContext>();
            MockMemberHttpContex.Setup(x => x.Request.Scheme).Returns("https");
            MockMemberHttpContex.Setup(x => x.Request.Host).Returns(myUri);
            MockMemberHttpContex.Setup(x => x.Request.Path).Returns("/users");
            _httpContext.HttpContext = MockMemberHttpContex.Object;

            var userController = new UserController(_userService);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            // Act
            var response = await userController.Get(paginationParams);
            var objectResult = response as ObjectResult;
            var result = objectResult.Value as Result<PagedResponse<UserDTO>>;

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task GetAllUsersFailThereAreNoUsersTest()
        {
            // Arrange
            DbContextHelper.MakeDbContext(false); // reinicializar DB sin datos
            var userController = new UserController(_userService);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            // Act
            var response = await userController.Get(paginationParams);
            var objectResult = response as ObjectResult;            

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);            
        }

        [TestMethod]
        public async Task PutUserSuccessfullyTest()
        {
            //Arrange
            var userDto = new UserUpdateDto()
            {
                FirstName = "testFirstname",
                LastName = "testLastname",
                Email = "testUser@ong.com",
                Photo = ImageHelper.GetImage()
            };

            var userController = new UserController(_userService);
            var login = new LoginContextHelper(_userService, _unitOfWork);
            userController.ControllerContext = await login.GetLoginContext("User12@ong.com", "Password12");

            //Act
            var response = await userController.Put(2, userDto);
            var result = response as ObjectResult;
            var expectedResult = result.Value as Result<UserDtoForDisplay>;

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(result);
            Assert.IsNotNull(expectedResult);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(expectedResult.Data, typeof(UserDtoForDisplay));
        }

        [TestMethod]
        public void PutUserFailRequiredFirstNameTest()
        {
            //Arrange
            var userDto = new UserUpdateDto()
            {
                FirstName = "",
                LastName = "testLastname",
                Email = "testUser@ong.com",
                Photo = ImageHelper.GetImage()
            };

            //Act
            var errorcount = checkValidationProperties(userDto).Count;

            //Assert
            Assert.AreNotEqual(0, errorcount);
        }

        [TestMethod]
        public void PutUserFailRequiredLastNameTest()
        {
            //Arrange
            var userDto = new UserUpdateDto()
            {
                FirstName = "testFirstName",
                LastName = "",
                Email = "testUser@ong.com",
                Photo = ImageHelper.GetImage()
            };

            //Act
            var errorcount = checkValidationProperties(userDto).Count;

            //Assert
            Assert.AreNotEqual(0, errorcount);
        }

        [TestMethod]
        public void PutUserFailRequiredEmailTest()
        {
            //Arrange
            var userDto = new UserUpdateDto()
            {
                FirstName = "testFirstName",
                LastName = "testLastname",
                Email = "",
                Photo = ImageHelper.GetImage()
            };

            //Act
            var errorcount = checkValidationProperties(userDto).Count;

            //Assert
            Assert.AreNotEqual(0, errorcount);
        }

        [TestMethod]
        public void PutUserFailInvalidFormatEmailTest()
        {
            //Arrange
            var userDto = new UserUpdateDto()
            {
                FirstName = "testFirstName",
                LastName = "testLastname",
                Email = "sdfa@",
                Photo = ImageHelper.GetImage()
            };

            //Act
            var errorcount = checkValidationProperties(userDto).Count;

            //Assert
            Assert.AreNotEqual(0, errorcount);
        }

        [TestMethod]
        public void PutUserFailRequiredPhotoTest()
        {
            //Arrange
            var userDto = new UserUpdateDto()
            {
                FirstName = "testFirstName",
                LastName = "testLastname",
                Email = "testUser@ong.com"
            };

            //Act
            var errorcount = checkValidationProperties(userDto).Count;

            //Assert
            Assert.AreNotEqual(0, errorcount);
        }

        public IList<ValidationResult> checkValidationProperties(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, result, true);
            if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);            

            return result;
        }
    }
}

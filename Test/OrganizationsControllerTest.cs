using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OngProject.Controllers;
using OngProject.Core.Business;
using OngProject.Core.Helper;
using OngProject.Core.Interfaces;
using OngProject.Core.Mapper;
using OngProject.Core.Models.DTOs;
using OngProject.Core.Models.Paged;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Response;
using OngProject.DataAccess;
using OngProject.Repositories;
using OngProject.Repositories.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Test.Helper;

namespace Test
{
    [TestClass]
    public class OrganizationsControllerTest
    {
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configurations;
        private JwtHelper _jwtHelper;
        private IUserService _userService;
        private IImageService _imageService;
        private IEntityMapper _entityMapper;
        private IOrganizationsService _organizationsService;
        private ISlideSerivice _slideService;
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
            _slideService = new SlideService(_unitOfWork, _entityMapper, _imageService, _httpContext);

            // Falta el Login
            _userService = new UsersService(_unitOfWork, _configurations, _jwtHelper, _imageService, _entityMapper, _httpContext);
            _organizationsService = new OrganizationService(_unitOfWork, _configurations, _jwtHelper, _entityMapper, _slideService, _imageService, _httpContext);
        }

        [TestMethod]
        public async Task AddOrganizationSuccesfullyTest()
        {
            //Arrange
            var Image = ImageHelper.GetImage();
            var organizationDTO = new OrganizationDTOForUpload
            {
                Name = "testName",
                AboutUsText = "About us test text",
                Address = "Adress test",
                FacebookUrl = "FacebookUrl",
                Email = "Email",
                Image = Image,
                InstagramUrl = "InstagramUrl",
                LinkedinUrl = "LinkedinUrl",
                Phone = 213123123,
                WelcomeText = "Welcome text test",

            };

            var organizationController = new OrganizationsController(_organizationsService);

            //Act
            var response = await organizationController.Post(organizationDTO);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<OrganizationDTOForDisplay>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultDTO);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(OrganizationDTOForDisplay));

        }

        [TestMethod]
        public async Task AddOrganizationWithNullNameFailedTest()
        {
            //Arrange
            var Image = ImageHelper.GetImage();
            var organizationDTO = new OrganizationDTOForUpload
            {
                Name = null,
                AboutUsText = "About us test text",
                Address = "Adress test",
                FacebookUrl = "FacebookUrl",
                Email = "Email",
                Image = Image,
                InstagramUrl = "InstagramUrl",
                LinkedinUrl = "LinkedinUrl",
                Phone = 213123123,
                WelcomeText = "Welcome text test",

            };
          
            var organizationController = new OrganizationsController(_organizationsService);

            //Act
            var response = await organizationController.Post(organizationDTO);
            var result = response as ObjectResult;

            //Assert
            Assert.AreNotEqual(0, result.StatusCode);

        }

        [TestMethod]
        public async Task AddOrganizationWithNullInstagramUrlTest()
        {
            //Arrange
            var Image = ImageHelper.GetImage();
            var organizationDTO = new OrganizationDTOForUpload
            {
                Name = null,
                AboutUsText = "About us test text",
                Address = "Adress test",
                FacebookUrl = "FacebookUrl",
                Email = "Email",
                Image = Image,
                InstagramUrl = null,
                LinkedinUrl = "LinkedinUrl",
                Phone = 213123123,
                WelcomeText = "Welcome text test",

            };
           
            var organizationController = new OrganizationsController(_organizationsService);

            //Act
            var response = await organizationController.Post(organizationDTO);
            var result = response as ObjectResult;

            //Assert
            Assert.AreNotEqual(0, result.StatusCode);

        }

        [TestMethod]

        public async Task PutOrganizationSuccesfullyTest()
        {
            //Arrange
            var Image = ImageHelper.GetImage();
            var organizationDTO = new OrganizationDTOForUpload()
            {
                Name = "testName",
                AboutUsText = "About us test text",
                Address = "Adress test",
                FacebookUrl = "FacebookUrl",
                Email = "Email",
                Image = Image,
                InstagramUrl = "InstagramUrl",
                LinkedinUrl = "LinkedinUrl",
                Phone = 213123123,
                WelcomeText = "Welcome text test",
            };

            var organizationController = new OrganizationsController(_organizationsService);

            //Act
            var response = await organizationController.Put( 1, organizationDTO);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<OrganizationDTOForDisplay>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultDTO);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(OrganizationDTOForDisplay));
        }

        [TestMethod]
        public void PutOrganizationFailRequiredName()
        {
            //Arrange
            var Image = ImageHelper.GetImage();
            var organizationDTO = new OrganizationDTOForUpload()
            {
                Name = "",
                AboutUsText = "About us test text",
                Address = "Adress test",
                FacebookUrl = "FacebookUrl",
                Email = "Email",
                Image = Image,
                InstagramUrl = "InstagramUrl",
                LinkedinUrl = "LinkedinUrl",
                Phone = 213123123,
                WelcomeText = "Welcome text test",
            };

            //Act
            var error = checkValidationProperties(organizationDTO).Count;

            //Assert
            Assert.AreNotEqual(0, error);
        }

        [TestMethod]
        public async Task GetAllOrganizationsSuccessfullyTest()
        {
            //Arrange
            HostString myUri = new HostString("www.api.com:8080");
            var MockMemberHttpContex = new Mock<HttpContext>();
            MockMemberHttpContex.Setup(x => x.Request.Scheme).Returns("https");
            MockMemberHttpContex.Setup(x => x.Request.Host).Returns(myUri);
            MockMemberHttpContex.Setup(x => x.Request.Path).Returns("/organizations");
            _httpContext.HttpContext = MockMemberHttpContex.Object;

            var organizationController = new OrganizationsController(_organizationsService);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            //Act
            var response = await organizationController.Get(paginationParams);
            var objectResult = response as ObjectResult;
            var result = objectResult.Value as Result<PagedResponse<OrganizationDTOForDisplay>>;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task GetAllOrganizationsWhenThereAreNoOrganizationsTest()
        {
            //Arrange
            var organizationController = new OrganizationsController(_organizationsService);
            DbContextHelper.MakeDbContext(false);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            //Act
            var response = await organizationController.Get(paginationParams);
            var result = response as ObjectResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
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


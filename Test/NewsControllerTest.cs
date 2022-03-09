using OngProject.Controllers;
using Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OngProject.Core.Business;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OngProject.Core.Models.Response;
using OngProject.Core.Models.DTOs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Moq;
using System.ComponentModel.DataAnnotations;
using OngProject.Core.Models.PagedResourceParameters;
using OngProject.Core.Models.Paged;
using OngProject.Core.Interfaces;
using OngProject.Core.Helper;
using OngProject.Core.Mapper;
using OngProject.Repositories;
using OngProject.DataAccess;
using Microsoft.Extensions.Configuration;
using OngProject.Repositories.Interfaces;

namespace Test
{
    [TestClass]
    public class NewsControllerTest
    {
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configurations;
        private JwtHelper _jwtHelper;
        private IUserService _userService;
        private IImageService _imageService;
        private IEntityMapper _entityMapper;
        private IHttpContextAccessor _httpContext;
        private INewsService _newsService;

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
            _newsService = new NewsService(_unitOfWork, _configurations, _entityMapper, _imageService, _httpContext);
        }

        [TestMethod]
        public async Task GetAllNewsSuccessfullyTest()
        {
            // Arrange          
            HostString myUri = new HostString("www.api.com:8080");
            var MockMemberHttpContex = new Mock<HttpContext>();
            MockMemberHttpContex.Setup(x => x.Request.Scheme).Returns("https");
            MockMemberHttpContex.Setup(x => x.Request.Host).Returns(myUri);
            MockMemberHttpContex.Setup(x => x.Request.Path).Returns("/news");
            _httpContext.HttpContext = MockMemberHttpContex.Object;

            var newsController = new NewsController(_newsService);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            // Act
            var response = await newsController.GetAllNews(paginationParams);
            var objectResult = response as ObjectResult;
            var result = objectResult.Value as Result<PagedResponse<NewDtoForDisplay>>;

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task GetAllNewsFailureThereAreNoNewsTest()
        {
            // Arrange
            DbContextHelper.MakeDbContext(false); // reinicializar DB sin datos
            var newsController = new NewsController(_newsService);

            // Act
            PaginationParams paginationParams = new PaginationParams();
            paginationParams.PageNumber = 1;
            paginationParams.PageSize = 5;
            var response = await newsController.GetAllNews(paginationParams);
            var objectResult = response as ObjectResult;

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task AddNewsSuccessfullyTest()
        {
            // Arrange     
            var Image = ImageHelper.GetImage();
            var newEntity = new NewDtoForUpload()
            {
                Name = "test name",
                Content = "test content",
                Image = Image,
                Category = 1,
            };
            
            var newsController = new NewsController(_newsService);

            // Act
            var response = await newsController.Post(newEntity);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<NewDtoForDisplay>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultDTO);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(NewDtoForDisplay));
        }

        [TestMethod]
        public async Task UpdateNewsSuccessfullyTest()
        {
            // Arrange
            var Image = ImageHelper.GetImage();
            int IdNew = 1;
            var newEntity = new NewDtoForUpload()
            {
                Name = "test name",
                Content = "test content",
                Image = Image,
                Category = 1,
            };

            var mockImageService = new Mock<ImageService>(_unitOfWork);
            var link = "https://www.netmentor.es/entrada/mock-unit-test-csharp";
            mockImageService.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<IFormFile>())).ReturnsAsync(link);
            mockImageService.Setup(x => x.AwsDeleteFile(It.IsAny<string>()));
            
            var newsController = new NewsController(_newsService);

            // Act
            var response = await newsController.Put(IdNew, newEntity);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<NewDtoForDisplay>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(NewDtoForDisplay));
        }

        [TestMethod]
        public async Task UpdateNewsFailureNonExistingNew()
        {
            // Arrange
            var Image = ImageHelper.GetImage();
            int IdNew = 100;
            var newEntity = new NewDtoForUpload()
            {
                Name = "test name",
                Content = "test content",
                Image = Image,
                Category = 1,
            };          
                       
            var newsController = new NewsController(_newsService);

            // Act
            var response = await newsController.Put(IdNew, newEntity);
            var result = response as ObjectResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteNewSuccessfullyTest()
        {
            // Arrange       
            var newsController = new NewsController(_newsService);
            var newEntity = await _unitOfWork.NewsRepository.GetByIdAsync(3);

            // Act
            var response = await newsController.Delete(newEntity.Id);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(newEntity.SoftDelete);
        }

        [TestMethod]
        public async Task DeleteNewNotFoundTest()
        {
            // Arrange            
            var newsController = new NewsController(_newsService);

            // Act
            var response = await newsController.Delete(100);
            var result = response as ObjectResult;

            // Assert
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
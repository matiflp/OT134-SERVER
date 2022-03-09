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
using OngProject.DataAccess;
using OngProject.Core.Helper;
using OngProject.Core.Mapper;
using OngProject.Repositories;
using OngProject.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OngProject.Repositories.Interfaces;

namespace Test
{
    [TestClass]
    public class TestimonialsControllerTest
    {
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configurations;
        private JwtHelper _jwtHelper;
        private IUserService _userService;
        private IImageService _imageService;
        private IEntityMapper _entityMapper;
        private IHttpContextAccessor _httpContext;
        private ITestimonialsService _testimonialsService;

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
            _testimonialsService = new TestimonialsService(_unitOfWork, _configurations, _entityMapper, _imageService, _httpContext);
        }

        [TestMethod]
        public async Task GetAllTestimonialsSuccessfullyTest()
        {
            // Arrange
            HostString myUri = new HostString("www.api.com:8080");
            var MockMemberHttpContex = new Mock<HttpContext>();
            MockMemberHttpContex.Setup(x => x.Request.Scheme).Returns("https");
            MockMemberHttpContex.Setup(x => x.Request.Host).Returns(myUri);
            MockMemberHttpContex.Setup(x => x.Request.Path).Returns("/testimonials");
            _httpContext.HttpContext = MockMemberHttpContex.Object;
            
            var testimonialsController = new TestimonialsController(_testimonialsService);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            // Act
            var response = await testimonialsController.GetAll(paginationParams);
            var objectResult = response as ObjectResult;
            var result = objectResult.Value as Result<PagedResponse<TestimonialDTODisplay>>;

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task GetAllTestimonialsFailureThereAreNoTestimonialsTest()
        {
            // Arrange
            HostString myUri = new HostString("www.api.com:8080");
            var MockMemberHttpContex = new Mock<HttpContext>();
            MockMemberHttpContex.Setup(x => x.Request.Scheme).Returns("https");
            MockMemberHttpContex.Setup(x => x.Request.Host).Returns(myUri);
            MockMemberHttpContex.Setup(x => x.Request.Path).Returns("/testimonials");
            _httpContext.HttpContext = MockMemberHttpContex.Object;

            DbContextHelper.MakeDbContext(false); // reinicializar DB sin datos
            var testimonialsController = new TestimonialsController(_testimonialsService);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            // Act
            var response = await testimonialsController.GetAll(paginationParams);
            var objectResult = response as ObjectResult;
            var result = objectResult.Value as Result<PagedResponse<TestimonialDTODisplay>>;

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task AddTestimonialsSuccessfullyTest()
        {
            // Arrange     
            var Image = ImageHelper.GetImage();
            var testimonialEntity = new TestimonialDTO()
            {
                Name = "test name",
                Content = "test content",
                File = Image,
            };

            var testimonialController = new TestimonialsController(_testimonialsService);

            // Act
            var response = await testimonialController.Post(testimonialEntity);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<TestimonialDTODisplay>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultDTO);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(TestimonialDTODisplay));
        }

        [TestMethod]
        public async Task UpdateTestimonialSuccessfullyTest()
        {
            // Arrange
            var Image = ImageHelper.GetImage();
            int IdTestimonial = 1;
            var testimonialEntity = new TestimonialDTO()
            {
                Name = "test name",
                Content = "test content",
                File = Image,
            };

            var testimonialController = new TestimonialsController(_testimonialsService);

            // Act
            var response = await testimonialController.Put(IdTestimonial, testimonialEntity);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<TestimonialDTODisplay>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(TestimonialDTODisplay));
        }

        [TestMethod]
        public async Task UpdateTestimonialsFailureNonExistingTestimonial()
        {
            // Arrange
            var Image = ImageHelper.GetImage();
            int IdTestimonial = 100;
            var testimonialEntity = new TestimonialDTO()
            {
                Name = "test name",
                Content = "test content",
                File = Image,
            };

            var testimonialController = new TestimonialsController(_testimonialsService);

            // Act
            var response = await testimonialController.Put(IdTestimonial, testimonialEntity);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteTestimonialSuccessfullyTest()
        {
            // Arrange
            var testimonialsController = new TestimonialsController(_testimonialsService);
            var testimonialEntity = await _unitOfWork.TestimonialsRepository.GetByIdAsync(3);

            // Act
            var response = await testimonialsController.Delete(testimonialEntity.Id);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(testimonialEntity.SoftDelete);
        }

        [TestMethod]
        public async Task DeleteTestimonialsNotFoundTest()
        {
            // Arrange            
            var testimonialsController = new TestimonialsController(_testimonialsService);

            // Act
            var response = await testimonialsController.Delete(100);
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
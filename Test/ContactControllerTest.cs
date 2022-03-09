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
using System.Threading.Tasks;
using Test.Helper;

namespace Test
{
    [TestClass]
    public class ContactControllerTest
    {
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configurations;
        private JwtHelper jwtHelper;
        private IUserService _userService;
        private IContactService _contactService;
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

            // FALTA AGREGAR EL LOGIN!!
            _userService = new UsersService(_unitOfWork, _configurations, jwtHelper, _imageService, _entityMapper, _httpContext);
            _contactService = new ContactService(_configurations, _unitOfWork, _entityMapper, _httpContext);
        }

        [TestMethod]
        public async Task GetAllSuccessTest()
        {
            // Arrange
            HostString myUri = new HostString("www.api.com:8080");
            var MockMemberHttpContex = new Mock<HttpContext>();
            MockMemberHttpContex.Setup(x => x.Request.Scheme).Returns("https");
            MockMemberHttpContex.Setup(x => x.Request.Host).Returns(myUri);
            MockMemberHttpContex.Setup(x => x.Request.Path).Returns("/contacts");
            _httpContext.HttpContext = MockMemberHttpContex.Object;

            var contactController = new ContactController(_contactService);
            PaginationParams paginationParams = new()
            {
                PageNumber = 1,
                PageSize = 5
            };

            //Act
            var response = await contactController.GetAllContacts(paginationParams);
            var objectResult = response as ObjectResult;
            var result = objectResult.Value as Result<PagedResponse<ContactDTO>>;

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task AddContactSuccessTest()
        {
            // Arrange
            var contactDto = new ContactDTO()
            {
                Email = "User12@ong.com",
                Message = "Mensaje de Prueba",
                Name = "Nombre de prueba",
                Phone = "12345678"
            };
            var contactController = new ContactController(_contactService);

            // Act
            var response = await contactController.Post(contactDto);
            var result = response as ObjectResult;
            var resultDto = result.Value as Result<ContactDTO>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDto.Data, typeof(ContactDTO));
        }

        [TestMethod]
        public async Task AddContactWithNullNameFailedTest()
        {
            // Arrange
            var contactDto = new ContactDTO()
            {
                Name = null,
                Email = "mailDePrueba@prueba.com",
                Message = "Mensaje de Prueba",
                Phone = "12345678"
            };     
            var contactController = new ContactController(_contactService);

            // Act
            var response = await contactController.Post(contactDto);
            var result = response as ObjectResult;

            // Assert
            Assert.AreNotEqual(0, result.StatusCode);
        }

        [TestMethod]
        public async Task AddContactWithNullEmailFailedTest()
        {
            // Arrange
            var contactDto = new ContactDTO()
            {
                Name = "Nombre de prueba",
                Email = null,
                Message = "Mensaje de Prueba",
                Phone = "12345678"
            };
            var contactController = new ContactController(_contactService);

            // Act
            var response = await contactController.Post(contactDto);
            var result = response as ObjectResult;

            // Assert
            Assert.AreNotEqual(0, result.StatusCode);
        }

    }
}

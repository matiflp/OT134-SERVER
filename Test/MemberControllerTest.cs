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
using System.Threading.Tasks;
using Test.Helper;

namespace Test
{
    [TestClass]
    public class MemberControllerTest
    {
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configurations;
        private JwtHelper _jwtHelper;
        private IUserService _userService;
        private IImageService _imageService;
        private IEntityMapper _entityMapper;
        private IMemberService _memberServie;
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

            // Falta el Login
            _userService = new UsersService(_unitOfWork, _configurations, _jwtHelper, _imageService, _entityMapper, _httpContext);
            _memberServie = new MemberService(_unitOfWork, _entityMapper, _httpContext, _imageService);
        }

        [TestMethod]
        public async Task DeleteMemeberSuccessfullyTest()
        {
            // Arrange            
            var mebmerController = new MembersController(_memberServie);
            var member = await _unitOfWork.MembersRepository.GetByIdAsync(3);

            // Act
            var response = await mebmerController.Delete(member.Id);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(member.SoftDelete);
        }
        
        [TestMethod]
        public async Task DeleteMemeberNotFoundTest()
        {
            // Arrange            
            var mebmerController = new MembersController(_memberServie);

            // Act
            var response = await mebmerController.Delete(100);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
        
        [TestMethod]
        public async Task DeleteMemeberAlreadyDeletedmemberTest()
        {
            // Arrange            
            var mebmerController = new MembersController(_memberServie);
            var member = await _unitOfWork.MembersRepository.GetByIdAsync(1);

            // Act
            var response = await mebmerController.Delete(member.Id);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsTrue(member.SoftDelete);
        }
        
        [TestMethod]
        public async Task GetAllMembersSuccessfullyTest()
        {
            // Arrange
            HostString myUri = new HostString("www.api.com:8080");
            var MockMemberHttpContex = new Mock<HttpContext>();
            MockMemberHttpContex.Setup(x => x.Request.Scheme).Returns("https");
            MockMemberHttpContex.Setup(x => x.Request.Host).Returns(myUri);
            MockMemberHttpContex.Setup(x => x.Request.Path).Returns("/Members");
            _httpContext.HttpContext = MockMemberHttpContex.Object;

            var memberController = new MembersController(_memberServie);

            // Act
            PaginationParams paginationParams = new PaginationParams();
            paginationParams.PageNumber = 1;
            paginationParams.PageSize = 5;
            var response = await memberController.GetAllMembers(paginationParams);
            var objectResult = response as ObjectResult;
            var result = objectResult.Value as Result<PagedResponse<MemberDTODisplay>>;

            // Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsTrue(result.Success);
        }
        
        [TestMethod]
        public async Task AddMemeberSuccessfullyTest()
        {
            // Arrange     
            var Image = ImageHelper.GetImage();

            var memberDTO = new MemberDTORegister()
            {
                Name = "nombre de prueba",
                Description = "Descripcion de prueba",
                File = Image
            };

            var mebmerController = new MembersController(_memberServie);

            // Act
            var response = await mebmerController.Post(memberDTO);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<MemberDTODisplay>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultDTO);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(MemberDTODisplay));
        }
        
        [TestMethod]
        public void AddingAMemberWithNullNameFailedTest()
        {
            // Arrange     
            var Image = ImageHelper.GetImage();

            var memberDTO = new MemberDTORegister()
            {
                Name = null,
                Description = "Descripcion de prueba",
                File = Image
            };

            // Act
            var errorcount = checkValidationProperties(memberDTO).Count;

            // Assert
            Assert.AreNotEqual(0, errorcount);
        }
        
        [TestMethod]
        public void AddingAMemberWithNullImageFailedTest()
        {
            // Arrange     
            var memberDTO = new MemberDTORegister()
            {
                Name = "nombre de prueba",
                Description = "Descripcion de prueba",
                File = null
            };
            // Act
            var errorcount = checkValidationProperties(memberDTO).Count;

            // Assert
            Assert.AreNotEqual(0, errorcount);
        }
        
        [TestMethod]
        public async Task AddingAnExistingMemberFailedTest()
        {
            // Arrange
            var Image = ImageHelper.GetImage();
            var memberDTO = new MemberDTORegister()
            {
                Name = "Name 1",
                Description = "Descripcion de prueba",
                File = Image
            };
         
            var mebmerController = new MembersController(_memberServie);

            // Act
            var response = await mebmerController.Post(memberDTO);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }
        
        [TestMethod]
        public async Task UpgradingASuccessulMemberTest()
        {
            // Arrange
            var Image = ImageHelper.GetImage();
            int IdMember = 2;
            var memberDTO = new MembersDtoForUpload()
            {
                Name = "Name 1 Test",
                Description = "Descripcion de prueba",
                Image = Image,
                FacebookUrl = "FacebookContact",
                InstagramUrl = "InstagramContact",
                LinkedinUrl = "LinkedinContact"
            };

            var mebmerController = new MembersController(_memberServie);

            // Act
            var response = await mebmerController.Put(IdMember, memberDTO);
            var result = response as ObjectResult;
            var resultDTO = result.Value as Result<MemberDTODisplay>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(resultDTO.Data, typeof(MemberDTODisplay));
        }
        
        [TestMethod]
        public async Task TryingToUpdatANonexistentMemberTest()
        {
            // Arrange
            var Image = ImageHelper.GetImage();
            int IdMember = 100;
            var memberDTO = new MembersDtoForUpload()
            {
                Name = "Name 1",
                Description = "Descripcion de prueba",
                Image = Image,
                FacebookUrl = "FacebookContact",
                InstagramUrl = "InstagramContact",
                LinkedinUrl = "LinkedinContact"
            };

            var mebmerController = new MembersController(_memberServie);

            // Act
            var response = await mebmerController.Put(IdMember, memberDTO);
            var result = response as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
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
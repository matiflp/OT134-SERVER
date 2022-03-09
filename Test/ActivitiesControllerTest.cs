using OngProject.Controllers;
using Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OngProject.Core.Business;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OngProject.Core.Models.DTOs;
using OngProject.Repositories.Interfaces;
using OngProject.DataAccess;
using OngProject.Repositories;
using OngProject.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using OngProject.Core.Mapper;

namespace Test
{
    [TestClass]
    public class ActivitiesControllerTest
    {
        #region TestInit and Injections
        private AppDbContext _context;
        private IUnitOfWork _unitOfWork;
        private IActivitiesService _activitiesService;
        private ActivitiesController _activitiesController;
        private IEntityMapper _entityMapper;
        private IImageService _imageService;
        private IHttpContextAccessor _httpContext;

        [TestInitialize]
        public void Init()
        {
            _context = DbContextHelper.MakeDbContext(true);
            _unitOfWork = new UnitOfWork(_context);

            _imageService = new ImageService(_unitOfWork);
            _entityMapper = new EntityMapper();
            _httpContext = new HttpContextAccessor();

            _activitiesService = new ActivitiesService(_unitOfWork, _entityMapper, _imageService, _httpContext);
            _activitiesController = new ActivitiesController(_activitiesService);                    
        }
        #endregion

        #region Tests
        [TestMethod]
        public async Task Post_ValidEntity_200Code()
        {
            //Arrange
            var newActivity = new ActivityDTOForRegister
            {
                Name = "Activity name",
                Content = "Activity content",
                file = ImageHelper.GetImage(),
            };

            //Act
            var response = await _activitiesController.Post(newActivity);
            var result = response as ObjectResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public async Task Post_NullEntity_400Code()
        {
            //Arrange
            ActivityDTOForRegister newActivity = null;           

            //Act
            var response = await _activitiesController.Post(newActivity);
            var result = response as ObjectResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public async Task Put_ValidEntity_200Code()
        {
            //Arrange
            var newActivity = new ActivitiesDtoForUpload
            {
                Name = "Activity name",
                Content = "Activity content",
                Image = ImageHelper.GetImage(),
            };

            //Act
            var response = await _activitiesController.Put(1, newActivity);
            var result = response as ObjectResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public async Task Put_EntityNotFound_404Code()
        {
            //Arrange
            var newActivity = new ActivitiesDtoForUpload
            {
                Name = "Activity name",
                Content = "Activity content",
                Image = ImageHelper.GetImage(),
            };

            //Act
            var response = await _activitiesController.Put((-1), newActivity);
            var result = response as ObjectResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
        #endregion
    }
}
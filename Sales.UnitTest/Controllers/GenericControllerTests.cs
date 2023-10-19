namespace Sales.UnitTest.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Sales.API.Controllers;
    using Sales.API.Data;
    using Sales.API.Interfaces;    
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;
    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The class GenericControllerTests
    /// </summary>

    [TestClass]
    public class GenericControllerTests
    {

        #region Attributes

        private readonly DbContextOptions<DataContext> _options;
        private readonly Mock<IGenericUnitOfWork<Category>> _unitOfWorkMock;

        #endregion Attributes

        #region Constructor

        public GenericControllerTests()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _unitOfWorkMock = new Mock<IGenericUnitOfWork<Category>>();
        }

        #endregion Constructor

        #region Methods

        [TestMethod]
        public async Task GetAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);
            var pagination = new PaginationDTO { Filter = "some" };

            /// Act
            var result = await controller.GetAsync(pagination) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetPagesAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);
            var pagination = new PaginationDTO { Filter = "Some" };

            /// Act
            var result = await controller.GetPagesAsync(pagination) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsNotFoundWhenEntityNotFound()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.GetAsync(1) as NotFoundResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsRecord()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var category = new Category { Id = 1, Name = "test" };
            var response = new Response<Category> { WasSuccess = true };
            _unitOfWorkMock.Setup(x => x.GetAsync(category.Id)).ReturnsAsync(category);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.GetAsync(1) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.GetAsync(category.Id), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task PostAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var category = new Category { Id = 1, Name = "test" };
            var response = new Response<Category> { WasSuccess = true };
            _unitOfWorkMock.Setup(x => x.AddAsync(category)).ReturnsAsync(response);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.PostAsync(category) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.AddAsync(category), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task PostAsync_ReturnsBadRequest()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var category = new Category { Id = 1, Name = "test" };
            var response = new Response<Category> { WasSuccess = false };
            _unitOfWorkMock.Setup(x => x.AddAsync(category)).ReturnsAsync(response);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.PostAsync(category) as BadRequestObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.AddAsync(category), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task PutAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var category = new Category { Id = 1, Name = "test" };
            var response = new Response<Category> { WasSuccess = true };
            _unitOfWorkMock.Setup(x => x.UpdateAsync(category)).ReturnsAsync(response);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.PutAsync(category) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.UpdateAsync(category), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task PutAsync_ReturnsBadRequest()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var category = new Category { Id = 1, Name = "test" };
            var response = new Response<Category> { WasSuccess = false };
            _unitOfWorkMock.Setup(x => x.UpdateAsync(category)).ReturnsAsync(response);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.PutAsync(category) as BadRequestObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.UpdateAsync(category), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsNoContentWhenEntityDeleted()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var category = new Category { Id = 1, Name = "test" };
            _unitOfWorkMock.Setup(x => x.GetAsync(category.Id)).ReturnsAsync(category);
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.DeleteAsync(category.Id) as NoContentResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.GetAsync(category.Id), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsNoContentWhenEntityNotFound()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var category = new Category { Id = 1, Name = "test" };
            var controller = new GenericController<Category>(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.DeleteAsync(category.Id) as NotFoundResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        #endregion Methods

    }
}
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

    #endregion Import

    /// <summary>
    /// The class StatesControllerTests
    /// </summary>

    [TestClass]
    public class StatesControllerTests
    {

        #region Attributes

        private readonly DbContextOptions<DataContext> _options;
        private readonly Mock<IGenericUnitOfWork<State>> _unitOfWorkMock;

        #endregion Attributes

        #region Constructor

        public StatesControllerTests()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _unitOfWorkMock = new Mock<IGenericUnitOfWork<State>>();
        }

        #endregion Constructor

        #region Methods

        [TestMethod]
        public async Task GetComboAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new StatesController(_unitOfWorkMock.Object, context);
            var countryId = 1;

            /// Act
            var result = await controller.GetComboAsync(countryId) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new StatesController(_unitOfWorkMock.Object, context);
            var pagination = new PaginationDTO { Id = 1, Filter = "Some" };

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
            var controller = new StatesController(_unitOfWorkMock.Object, context);
            var pagination = new PaginationDTO { Id = 1, Filter = "Some" };

            /// Act
            var result = await controller.GetPagesAsync(pagination) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsNotFoundWhenStateNotFound()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new StatesController(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.GetAsync(1) as NotFoundResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsOkWhenStateFound()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var state = new State { Id = 1, Name = "test" };
            _unitOfWorkMock.Setup(x => x.GetStateAsync(state.Id)).ReturnsAsync(state);
            var controller = new StatesController(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.GetAsync(state.Id) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.GetStateAsync(state.Id), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        #endregion Methods

    }
}
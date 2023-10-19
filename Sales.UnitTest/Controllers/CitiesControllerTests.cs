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
    /// The class CitiesControllerTests
    /// </summary>

    [TestClass]
    public class CitiesControllerTests
    {

        #region Attributes

        private readonly DbContextOptions<DataContext> _options;
        private readonly Mock<IGenericUnitOfWork<City>> _unitOfWorkMock;

        #endregion Attributes

        #region Constructor

        public CitiesControllerTests()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _unitOfWorkMock = new Mock<IGenericUnitOfWork<City>>();
        }

        #endregion Constructor

        #region Methods

        [TestMethod]
        public async Task GetComboAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new CitiesController(_unitOfWorkMock.Object, context);
            var stateId = 1;

            /// Act
            var result = await controller.GetComboAsync(stateId) as OkObjectResult;

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
            var controller = new CitiesController(_unitOfWorkMock.Object, context);
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
            var controller = new CitiesController(_unitOfWorkMock.Object, context);
            var pagination = new PaginationDTO { Id = 1, Filter = "Some" };

            /// Act
            var result = await controller.GetPagesAsync(pagination) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
        }

        #endregion Methods

    }
}
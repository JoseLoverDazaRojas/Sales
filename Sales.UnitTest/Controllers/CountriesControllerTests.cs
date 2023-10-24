namespace Sales.UnitTest.Controllers
{

    #region Import

    using System;
    using System.Threading.Tasks;
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
    /// The class CountriesControllerTests
    /// </summary>

    [TestClass]
    public class CountriesControllerTests
    {

        #region Attributes

        private readonly DbContextOptions<DataContext> _options;
        private readonly Mock<IGenericUnitOfWork<Country>> _unitOfWorkMock;

        #endregion Attributes

        #region Constructor

        public CountriesControllerTests()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _unitOfWorkMock = new Mock<IGenericUnitOfWork<Country>>();
        }

        #endregion Constructor

        #region Methods

        [TestMethod]
        public async Task GetComboAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new CountriesController(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.GetComboAsync() as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new CountriesController(_unitOfWorkMock.Object, context);
            var pagination = new PaginationDTO { Filter = "some" };

            /// Act
            var result = await controller.GetAsync(pagination) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public async Task GetPagesAsync_ReturnsOkResult()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new CountriesController(_unitOfWorkMock.Object, context);
            var pagination = new PaginationDTO { Filter = "some" };

            /// Act
            var result = await controller.GetPagesAsync(pagination) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsNotFoundWhenCountryNotFound()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var controller = new CountriesController(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.GetAsync(1) as NotFoundResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_ReturnsOkWhenCountryFound()
        {
            /// Arrange
            using var context = new DataContext(_options);
            var country = new Country { Id = 1, Name = "test" };
            _unitOfWorkMock.Setup(x => x.GetCountryAsync(country.Id)).ReturnsAsync(country);
            var controller = new CountriesController(_unitOfWorkMock.Object, context);

            /// Act
            var result = await controller.GetAsync(country.Id) as OkObjectResult;

            /// Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            _unitOfWorkMock.Verify(x => x.GetCountryAsync(country.Id), Times.Once());

            /// Clean up (if needed)
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        #endregion Methods

    }
}
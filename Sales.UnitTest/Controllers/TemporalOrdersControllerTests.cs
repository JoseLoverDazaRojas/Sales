namespace Sales.UnitTest.Controllers
{

    #region Import

    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Sales.API.Controllers;
    using Sales.API.Data;
    using Sales.API.Interfaces;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;
    using System.Security.Principal;
    using Sales.UnitTest.Shared;
    using Moq;

    #endregion Import

    /// <summary>
    /// The class TemporalOrdersControllerTests
    /// </summary>

    [TestClass]
    public class TemporalOrdersControllerTests
    {

        #region Attributes

        private TemporalOrdersController _controller;
        private DataContext _context;
        private DbContextOptions<DataContext> _options;

        #endregion Attributes

        #region Constructor

        #endregion Constructor

        #region Methods

        [TestInitialize]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new DataContext(_options);
            var mockUnitOfWork = new Mock<IGenericUnitOfWork<TemporalOrder>>();
            _controller = new TemporalOrdersController(mockUnitOfWork.Object, _context);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@example.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        [TestCleanup]
        public void CleanUp()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnTemporalOrder_WhenIdExists()
        {
            /// Arrange
            var expectedOrder = new TemporalOrder { Id = 1 };
            _context.TemporalOrders.Add(expectedOrder);
            await _context.SaveChangesAsync();

            /// Act
            var result = await _controller.GetAsync(1);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
        }

        [TestMethod]
        public async Task PutAsync_ShouldUpdateTemporalOrder_WhenOrderExists()
        {
            /// Arrange
            var existingOrder = new TemporalOrder { Id = 1, Remarks = "Initial Remarks", Quantity = 10 };
            _context.TemporalOrders.Add(existingOrder);
            await _context.SaveChangesAsync();

            var dto = new TemporalOrderDTO { Id = 1, Remarks = "Updated Remarks", Quantity = 20 };

            /// Act
            var result = await _controller.PutAsync(dto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedDto = okResult!.Value as TemporalOrderDTO;
            Assert.AreEqual(dto.Remarks, returnedDto!.Remarks);
            Assert.AreEqual(dto.Quantity, returnedDto.Quantity);
        }

        [TestMethod]
        public async Task PutAsync_ShouldReturnNotFound_WhenTemporalOrderDoesNotExistAsync()
        {
            /// Arrange
            var dto = new TemporalOrderDTO { Id = 1, Remarks = "Some Remarks", Quantity = 5 };

            /// Act
            var result = await _controller.PutAsync(dto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostAsync_ShouldAddTemporalOrder_WhenDataIsValid()
        {
            /// Arrange
            var product = new Product { Id = 1, Name = "Some", Description = "Some" };
            var user = new User { Email = "test@example.com", Address = "Some", Document = "Some", FirstName = "John", LastName = "Doe" };
            _context.Products.Add(product);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new TemporalOrderDTO { ProductId = 1, Remarks = "New Remarks", Quantity = 10 };

            /// Act
            var result = await _controller.PostAsync(dto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task PostAsync_ShouldReturnNotFound_WhenProductDoesNotExistAsync()
        {
            /// Arrange
            var dto = new TemporalOrderDTO { ProductId = 1 };

            /// Act
            var result = await _controller.PostAsync(dto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostAsync_ShouldReturnNotFound_WhenUserDoesNotExistAsync()
        {
            /// Arrange
            var product = new Product { Id = 1, Name = "Some", Description = "Some" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var dto = new TemporalOrderDTO { ProductId = 1 };

            /// Act
            var result = await _controller.PostAsync(dto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostAsync_ShouldAddTemporalOrder_WhenProductAndUserExistAsync()
        {
            /// Arrange
            var product = new Product { Id = 1, Name = "Some", Description = "Some" };
            _context.Products.Add(product);

            var user = new User { Email = "test@example.com", Address = "Some", Document = "Some", FirstName = "John", LastName = "Doe" };
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var dto = new TemporalOrderDTO { ProductId = 1, Remarks = "Test Remarks", Quantity = 5 };

            /// Act
            var result = await _controller.PostAsync(dto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedDto = okResult!.Value as TemporalOrderDTO;
            Assert.AreEqual(dto.Remarks, returnedDto!.Remarks);
            Assert.AreEqual(dto.Quantity, returnedDto.Quantity);
        }

        [TestMethod]
        public async Task PostAsync_ExceptionThrown_ReturnsBadRequest()
        {
            /// Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var exceptionalContext = new ExceptionalDataContext(options);

            exceptionalContext.Products.Add(new Product { Id = 1, Name = "Some", Description = "Some" });
            exceptionalContext.Users.Add(new User { Email = "test@example.com", Address = "Some", Document = "Some", FirstName = "John", LastName = "Doe" });
            exceptionalContext.SaveChanges();

            var controller = CreateControllerWithMockedUserEmail("test@test.com", exceptionalContext);

            var temporalOrderDTO = new TemporalOrderDTO { ProductId = 1, Quantity = 5, Remarks = "TestRemarks" };

            /// Act
            var result = await controller.PostAsync(temporalOrderDTO);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Test Exception", ((BadRequestObjectResult)result).Value);
        }

        private TemporalOrdersController CreateControllerWithMockedUserEmail(string email, DataContext context)
        {
            var mockIdentity = new Mock<IIdentity>();
            mockIdentity.Setup(x => x.Name).Returns(email);
            var principal = new Mock<IPrincipal>();
            principal.Setup(x => x.Identity).Returns(mockIdentity.Object);

            var controller = new TemporalOrdersController(null, context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = (ClaimsPrincipal)principal.Object }
            };
            return controller;
        }

        #endregion Methods

    }
}
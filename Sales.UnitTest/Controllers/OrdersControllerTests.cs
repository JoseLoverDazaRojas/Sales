namespace Sales.UnitTest.Controllers
{

    #region Import

    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Sales.API.Controllers;
    using Sales.API.Data;
    using Sales.API.Helpers.Interfaces;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;
    using Sales.Shared.Enums;
    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The class OrdersControllerTests
    /// </summary>

    [TestClass]
    public class OrdersControllerTests
    {

        #region Attributes

        private OrdersController _controller = null!;
        private Mock<IOrdersHelper> _mockOrdersHelper = null!;
        private DataContext _mockDbContext = null!;
        private Mock<IUserHelper> _mockUserHelper = null!;

        #endregion Attributes

        #region Methods

        [TestInitialize]
        public void SetUp()
        {
            _mockOrdersHelper = new Mock<IOrdersHelper>();
            _mockUserHelper = new Mock<IUserHelper>();

            /// Setting up InMemory database
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            _mockDbContext = new DataContext(options);

            _controller = new OrdersController(_mockOrdersHelper.Object, _mockDbContext, _mockUserHelper.Object);

            /// Mock user identity
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
        public void Cleanup()
        {
            _mockDbContext.Database.EnsureDeleted();
            _mockDbContext.Dispose();
        }

        [TestMethod]
        public async Task PutAsync_OrderNotFound_ReturnsNotFound()
        {
            /// Arrange
            var orderDto = new OrderDTO { Id = 1 };
            var userName = "test@example.com";
            var user = new User { UserName = userName, UserType = UserType.Admin };
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(user);
            _mockUserHelper.Setup(x => x.IsUserInRoleAsync(user, user.UserType.ToString()))
                .ReturnsAsync(true);

            /// Act
            var result = await _controller.PutAsync(orderDto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.IsUserInRoleAsync(user, UserType.Admin.ToString()), Times.Once());
        }

        [TestMethod]
        public async Task PutAsync_UserNotFound_ReturnsNotFound()
        {
            /// Act
            var result = await _controller.PutAsync(new OrderDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PutAsync_NoAdmin_ReturnsBadRequest()
        {
            /// Arrange
            var orderDto = new OrderDTO { Id = 1 };
            var userName = "test@example.com";
            var user = new User { UserName = userName, UserType = UserType.User };
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(user);
            _mockUserHelper.Setup(x => x.IsUserInRoleAsync(user, user.UserType.ToString()))
                .ReturnsAsync(false);

            /// Act
            var result = await _controller.PutAsync(orderDto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.IsUserInRoleAsync(user, UserType.Admin.ToString()), Times.Once());
        }

        [TestMethod]
        public async Task PutAsync_ValidParameters_ReturnsOk()
        {
            /// Arrange
            var orderDto = new OrderDTO { Id = 1, OrderStatus = OrderStatus.Cancelled };
            var userName = "test@example.com";
            var user = new User { UserName = userName, UserType = UserType.Admin };
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(user);
            _mockUserHelper.Setup(x => x.IsUserInRoleAsync(user, user.UserType.ToString()))
                .ReturnsAsync(true);

            _mockDbContext.Products.Add(new Product
            {
                Id = 1,
                Name = "Some",
                Description = "Some"
            });
            _mockDbContext.Orders.Add(new Order
            {
                Id = orderDto.Id,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                }
            });
            await _mockDbContext.SaveChangesAsync();

            /// Act
            var result = await _controller.PutAsync(orderDto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.IsUserInRoleAsync(user, UserType.Admin.ToString()), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_OrderNotFound_ReturnsNotFound()
        {
            /// Act
            var result = await _controller.GetAsync(1);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetAsync_OrderFound_ReturnsOk()
        {
            /// Arrange
            _mockDbContext.Products.Add(new Product
            {
                Id = 1,
                Name = "Some",
                Description = "Some"
            });
            _mockDbContext.Orders.Add(new Order
            {
                Id = 1,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                }
            });
            await _mockDbContext.SaveChangesAsync();

            /// Act
            var result = await _controller.GetAsync(1);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetAsync_UserNotValid_ReturnsBadRequest()
        {
            /// Act
            var result = await _controller.GetAsync(new PaginationDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetAsync_ValidParameters_ReturnsOk()
        {
            /// Arrange
            var userName = "test@example.com";
            var user = new User { UserName = userName, UserType = UserType.User };
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(user);
            _mockUserHelper.Setup(x => x.IsUserInRoleAsync(user, user.UserType.ToString()))
                .ReturnsAsync(false);

            /// Act
            var result = await _controller.GetAsync(new PaginationDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.IsUserInRoleAsync(user, UserType.Admin.ToString()), Times.Once());
        }

        [TestMethod]
        public async Task GetPagesAsync_UserNotValid_ReturnsBadRequest()
        {
            /// Act
            var result = await _controller.GetPagesAsync(new PaginationDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetPagesAsync_ValidParameters_ReturnsOk()
        {
            /// Arrange
            var userName = "test@example.com";
            var user = new User { UserName = userName, UserType = UserType.User };
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(user);
            _mockUserHelper.Setup(x => x.IsUserInRoleAsync(user, user.UserType.ToString()))
                .ReturnsAsync(false);

            /// Act
            var result = await _controller.GetPagesAsync(new PaginationDTO());

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            double totalPages = (double)okResult.Value!;
            Assert.AreEqual(0, totalPages);
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.IsUserInRoleAsync(user, UserType.Admin.ToString()), Times.Once());
        }

        [TestMethod]
        public async Task PostAsync_ProcessOrderSuccess_ReturnsNoContent()
        {
            /// Arrange
            var response = new Response<bool> { WasSuccess = true };
            _mockOrdersHelper.Setup(o => o.ProcessOrderAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            /// Act
            var result = await _controller.PostAsync(new OrderDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockOrdersHelper.Verify(x => x.ProcessOrderAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task PostAsync_ProcessOrderNotSuccess_ReturnsBadRequest()
        {
            /// Arrange
            var response = new Response<bool> { WasSuccess = false };
            _mockOrdersHelper.Setup(o => o.ProcessOrderAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            /// Act
            var result = await _controller.PostAsync(new OrderDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockOrdersHelper.Verify(x => x.ProcessOrderAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        #endregion Methods

    }
}
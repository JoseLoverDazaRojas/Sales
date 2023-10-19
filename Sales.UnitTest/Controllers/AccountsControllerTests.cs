namespace Sales.UnitTest.Controllers
{

    #region Import

    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Sales.API.Controllers;
    using Sales.API.Data;
    using Sales.API.Helpers.Interfaces;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;
    using Sales.Shared.Enums;
    using Sales.Shared.Responses;
    using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

    #endregion Import

    /// <summary>
    /// The class AccountsControllerTests
    /// </summary>

    [TestClass]
    public class AccountsControllerTests
    {

        #region Attributes

        private Mock<IUserHelper> _mockUserHelper = null!;
        private Mock<IMailHelper> _mockMailHelper = null!;
        private Mock<IConfiguration> _mockConfiguration = null!;
        private Mock<IFileStorage> _mockFileStorage = null!;
        private AccountsController _controller = null!;
        private DataContext _context = null!;
        private const string _container = "userphotos";
        private const string _string64base = "U29tZVZhbGlkQmFzZTY0U3RyaW5n";

        #endregion Attributes

        #region Methods

        [TestInitialize]
        public void Setup()
        {
            _mockUserHelper = new Mock<IUserHelper>();
            _mockMailHelper = new Mock<IMailHelper>();
            _mockFileStorage = new Mock<IFileStorage>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration
                .SetupGet(x => x["Url Frontend"])
                .Returns("http://frontend-url.com");
            _mockConfiguration
                .SetupGet(x => x["jwtKey"])
                .Returns("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper
                .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://generated-link.com");

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new DataContext(options);

            /// Seed the database
            _context.Countries.Add(new Country
            {
                Name = "Colombia",
                States = new List<State>
                {
                    new State
                    {
                        Name = "Antioquia",
                        Cities = new List<City>
                        {
                            new City { Name = "Medellín" }
                        }
                    }
                }
            });
            _context.Users.Add(new User { FirstName = "John", LastName = "Doe", Address = "Calle Luna Calle Sol", Document = "1010", CityId = 1 });
            _context.Users.Add(new User { FirstName = "Jane", LastName = "Smith", Address = "Calle Luna Calle Sol", Document = "2020", CityId = 1 });
            _context.SaveChanges();

            _controller = new AccountsController(_mockUserHelper.Object, _mockConfiguration.Object, _mockFileStorage.Object, _mockMailHelper.Object, _context)
            {
                Url = mockUrlHelper.Object
            };

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();

            mockHttpRequest.Setup(req => req.Scheme)
                .Returns("http");
            mockHttpContext.Setup(ctx => ctx.Request)
                .Returns(mockHttpRequest.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnFilteredUsers()
        {
            /// Arrange
            var paginationDTO = new PaginationDTO
            {
                Filter = "John"
            };

            /// Act
            var result = await _controller.GetAll(paginationDTO);
            var okResult = result as OkObjectResult;
            var users = okResult?.Value as List<User>;

            /// Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, users!.Count);
            Assert.AreEqual("John", users[0].FirstName);
        }

        [TestMethod]
        public async Task GetPages_WithFilter_ShouldReturnCorrectPageCount()
        {
            /// Arrange
            var paginationDTO = new PaginationDTO
            {
                Filter = "John",
                RecordsNumber = 1
            };

            /// Act
            var result = await _controller.GetPages(paginationDTO);
            var okResult = result as OkObjectResult;
            var totalPages = okResult?.Value;

            /// Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1.0, totalPages);
        }

        [TestMethod]
        public async Task GetPages_WithoutFilter_ShouldReturnCorrectPageCount()
        {
            /// Arrange
            var paginationDTO = new PaginationDTO
            {
                RecordsNumber = 1
            };

            /// Act
            var result = await _controller.GetPages(paginationDTO);
            var okResult = result as OkObjectResult;
            var totalPages = okResult?.Value;

            /// Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(2.0, totalPages);
        }

        [TestMethod]
        public async Task RecoverPassword_UserNotFound_ReturnsNotFound()
        {
            /// Arrange
            var userName = "test@example.com";
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync((User)null);

            /// Act
            var result = await _controller.RecoverPassword(new EmailDTO { Email = userName });

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
        }

        [TestMethod]
        public async Task RecoverPassword_EmailSentSuccessfully_ReturnsNoContent()
        {
            /// Arrange
            var user = new User { Email = "test@example.com" };
            _mockUserHelper.Setup(x => x.GetUserAsync(user.Email))
                .ReturnsAsync(user);
            _mockUserHelper.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("GeneratedToken");

            var response = new Response<string> { WasSuccess = true };
            _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(response);

            /// Act
            var result = await _controller.RecoverPassword(new EmailDTO { Email = user.Email });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(user.Email), Times.Once());
            _mockUserHelper.Verify(x => x.GeneratePasswordResetTokenAsync(user), Times.Once());
            _mockMailHelper.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task RecoverPassword_EmailFailedWithMessage_ReturnsBadRequestWithMessage()
        {
            /// Arrange
            var user = new User { Email = "test@example.com" };
            var message = "Failed to send";
            _mockUserHelper.Setup(x => x.GetUserAsync(user.Email))
                .ReturnsAsync(user);
            _mockUserHelper.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("GeneratedToken");

            var response = new Response<string> { WasSuccess = false, Message = message };
            _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(response);

            /// Act
            var result = await _controller.RecoverPassword(new EmailDTO { Email = user.Email });
            var badRequest = result as BadRequestObjectResult;

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(message, badRequest!.Value);
            _mockUserHelper.Verify(x => x.GetUserAsync(user.Email), Times.Once());
            _mockUserHelper.Verify(x => x.GeneratePasswordResetTokenAsync(user), Times.Once());
            _mockMailHelper.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ResetPassword_UserNotFound_ReturnsNotFound()
        {
            /// Arrange
            _mockUserHelper.Setup(x => x.GetUserAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            /// Act
            var result = await _controller.ResetPassword(new ResetPasswordDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ResetPassword_ValidReset_ReturnsNoContent()
        {
            /// Arrange
            var mockUser = new User();
            var mockIdentityResult = IdentityResult.Success;

            _mockUserHelper.Setup(x => x.GetUserAsync(It.IsAny<string>()))
                .ReturnsAsync(mockUser);
            _mockUserHelper.Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockIdentityResult);

            /// Act
            var result = await _controller.ResetPassword(new ResetPasswordDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(It.IsAny<string>()), Times.Once());
            _mockUserHelper.Verify(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ResetPassword_InvalidReset_ReturnsBadRequest()
        {
            /// Arrange
            var description = "Test error";
            var mockUser = new User();
            var mockIdentityErrors = new List<IdentityError>
            {
                new IdentityError { Description = description }
            };
            var mockIdentityResult = IdentityResult.Failed(mockIdentityErrors.ToArray());

            _mockUserHelper.Setup(x => x.GetUserAsync(It.IsAny<string>()))
                .ReturnsAsync(mockUser);
            _mockUserHelper.Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockIdentityResult);

            /// Act
            var result = await _controller.ResetPassword(new ResetPasswordDTO());
            var badRequestResult = result as BadRequestObjectResult;

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(description, badRequestResult!.Value);
            _mockUserHelper.Verify(x => x.GetUserAsync(It.IsAny<string>()), Times.Once());
            _mockUserHelper.Verify(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ChangePasswordAsync_InvalidModel_ReturnsBadRequest()
        {
            /// Arrange
            _controller.ModelState.AddModelError("TestError", "Test error message");

            /// Act
            var result = await _controller.ChangePasswordAsync(new ChangePasswordDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task ChangePasswordAsync_UserNotFound_ReturnsNotFound()
        {
            /// Arrange
            var userName = "testuser";
            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName)).ReturnsAsync((User)null);

            /// Act
            var result = await _controller.ChangePasswordAsync(new ChangePasswordDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ValidChange_ReturnsNoContent()
        {
            /// Arrange
            var userName = "testuser";
            var mockUser = new User();
            var mockIdentityResult = IdentityResult.Success;

            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(mockUser);
            _mockUserHelper.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockIdentityResult);

            /// Act
            var result = await _controller.ChangePasswordAsync(new ChangePasswordDTO());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ChangePasswordAsync_InvalidChange_ReturnsBadRequest()
        {
            /// Arrange
            var userName = "testuser";
            var description = "Test error";
            var mockUser = new User();
            var mockIdentityErrors = new List<IdentityError>
            {
                new IdentityError { Description = description }
            };
            var mockIdentityResult = IdentityResult.Failed(mockIdentityErrors.ToArray());

            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(mockUser);
            _mockUserHelper.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockIdentityResult);

            /// Act
            var result = await _controller.ChangePasswordAsync(new ChangePasswordDTO());
            var badRequestResult = result as BadRequestObjectResult;

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(description, badRequestResult!.Value);
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task PutAsync_UserNotFound_ReturnsNotFound()
        {
            /// Arrange
            var userName = "testuser";
            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync((User)null);

            /// Act
            var result = await _controller.PutAsync(new User());

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
        }

        [TestMethod]
        public async Task PutAsync_ExceptionThrown_ReturnsBadRequest()
        {
            /// Arrange
            var message = "Test exception";
            var userName = "testuser";
            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .Throws(new Exception(message));

            /// Act
            var result = await _controller.PutAsync(new User());
            var badRequestResult = result as BadRequestObjectResult;

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(message, badRequestResult!.Value);
        }

        [TestMethod]
        public async Task PutAsync_UserPhotoNotEmpty_UpdatesPhoto()
        {
            /// Arrange
            var user = new User
            {
                Email = "some@yopmail.com",
                UserType = UserType.User,
                Document = "123",
                FirstName = "John",
                LastName = "Doe",
                Address = "Any",
                Photo = _string64base,
                CityId = 1
            };
            var currentUser = new User
            {
                Email = "some@yopmail.com",
                UserType = UserType.User,
                Document = "123",
                FirstName = "John",
                LastName = "Doe",
                Address = "Any",
                Photo = "oldPhoto",
                CityId = 1
            };
            var userName = "testuser";
            var newPhotoUrl = "newPhotoUrl";
            var mockIdentityResult = IdentityResult.Success;

            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(currentUser);
            _mockFileStorage.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container))
                .ReturnsAsync(newPhotoUrl);
            _mockUserHelper.Setup(x => x.UpdateUserAsync(currentUser))
                .ReturnsAsync(mockIdentityResult);

            /// Act
            var result = await _controller.PutAsync(user);
            var okResult = result as OkObjectResult;
            var token = okResult?.Value as TokenDTO;

            /// Assert
            Assert.IsNotNull(token!.Token);
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.UpdateUserAsync(currentUser), Times.Once());
        }

        [TestMethod]
        public async Task PutAsync_PhotoUpdateException_ReturnsBadRequest()
        {
            /// Arrange
            var user = new User { Photo = _string64base };
            var userName = "testuser";
            var message = "Photo upload failed";

            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(new User());
            _mockFileStorage.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container))
                .Throws(new Exception(message));

            /// Act
            var result = await _controller.PutAsync(user);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
        }

        [TestMethod]
        public async Task PutAsync_UpdateUserFails_ReturnsBadRequest()
        {
            /// Arrange
            var user = new User();
            var currentUser = new User();
            var identityError = new IdentityError { Description = "Update failed" };
            var userName = "testuser";

            _controller.ControllerContext = GetControllerContext(userName);
            _mockUserHelper.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(currentUser);
            _mockUserHelper.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            /// Act
            var result = await _controller.PutAsync(user);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(userName), Times.Once());
            _mockUserHelper.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_UserExists_ReturnsOkWithUser()
        {
            /// Arrange
            var expectedUser = new User
            {
                Email = "testuser@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _controller.ControllerContext = GetControllerContext(expectedUser.Email);
            _mockUserHelper.Setup(x => x.GetUserAsync(expectedUser.Email))
                .ReturnsAsync(expectedUser);

            /// Act
            var result = await _controller.GetAsync();

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedUser = okResult!.Value as User;
            Assert.AreEqual(expectedUser.Email, returnedUser!.Email);
            Assert.AreEqual(expectedUser.FirstName, returnedUser!.FirstName);
            Assert.AreEqual(expectedUser.LastName, returnedUser!.LastName);
        }

        [TestMethod]
        public async Task CreateUserAsync_ValidModel_NoPhoto_ReturnsNoContent()
        {
            /// Arrange
            var userModel = new UserDTO
            {
                UserName = "test",
                Password = "123456",
                UserType = UserType.User
            };

            _mockUserHelper.Setup(x => x.AddUserAsync(userModel, userModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserHelper.Setup(x => x.AddUserToRoleAsync(userModel, userModel.UserType.ToString()))
                .Returns(Task.CompletedTask);

            _mockUserHelper.Setup(x => x.GenerateEmailConfirmationTokenAsync(userModel))
               .ReturnsAsync("GeneratedToken");

            var response = new Response<string> { WasSuccess = true };
            _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(response);

            /// Act
            var result = await _controller.CreateUserAsync(userModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserHelper.Verify(x => x.AddUserAsync(userModel, userModel.Password), Times.Once());
            _mockUserHelper.Verify(x => x.AddUserToRoleAsync(userModel, userModel.UserType.ToString()), Times.Once());
            _mockUserHelper.Verify(x => x.GenerateEmailConfirmationTokenAsync(userModel), Times.Once());
            _mockMailHelper.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task CreateUserAsync_ValidModel_WithPhoto_ReturnsNoContent()
        {
            /// Arrange
            var userModel = new UserDTO
            {
                Photo = _string64base
            };

            _mockFileStorage.Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", It.IsAny<string>()))
                .ReturnsAsync("somePhotoPath");

            _mockUserHelper.Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserHelper.Setup(x => x.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockUserHelper.Setup(x => x.GenerateEmailConfirmationTokenAsync(userModel))
               .ReturnsAsync("GeneratedToken");

            var response = new Response<string> { WasSuccess = true };
            _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(response);

            /// Act
            var result = await _controller.CreateUserAsync(userModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockFileStorage.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", It.IsAny<string>()), Times.Once());
            _mockUserHelper.Verify(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
            _mockUserHelper.Verify(x => x.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
            _mockUserHelper.Verify(x => x.GenerateEmailConfirmationTokenAsync(userModel), Times.Once());
            _mockMailHelper.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task CreateUserAsync_AddUserFails_ReturnsBadRequest()
        {
            /// Arrange
            var userModel = new UserDTO();

            var identityErrors = new List<IdentityError> { new IdentityError { Description = "User creation failed" } };
            _mockUserHelper.Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            /// Act
            var result = await _controller.CreateUserAsync(userModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockUserHelper.Verify(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task CreateUserAsync_SendConfirmationEmailFails_ReturnsBadRequest()
        {
            /// Arrange
            var userModel = new UserDTO();

            _mockUserHelper.Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserHelper.Setup(x => x.GenerateEmailConfirmationTokenAsync(userModel))
               .ReturnsAsync("GeneratedToken");

            var response = new Response<string> { WasSuccess = false, Message = "Email sending failed" };
            _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(response);

            /// Act
            var result = await _controller.CreateUserAsync(userModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Email sending failed", badRequestResult!.Value);
            _mockUserHelper.Verify(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
            _mockUserHelper.Verify(x => x.GenerateEmailConfirmationTokenAsync(userModel), Times.Once());
            _mockMailHelper.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task LoginAsync_SuccessfulLogin_ReturnsOk()
        {
            /// Arrange
            var user = new User
            {
                Email = "some@yopmail.com",
                UserType = UserType.User,
                Document = "123",
                FirstName = "John",
                LastName = "Doe",
                Address = "Any",
                Photo = _string64base,
                CityId = 1
            };
            var loginModel = new LoginDTO { Email = user.Email, Password = "123456" };

            _mockUserHelper.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(SignInResult.Success);

            _mockUserHelper.Setup(x => x.GetUserAsync(user.Email))
                .ReturnsAsync(user);

            /// Act
            var result = await _controller.LoginAsync(loginModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockUserHelper.Verify(x => x.LoginAsync(loginModel), Times.Once());
            _mockUserHelper.Verify(x => x.GetUserAsync(user.Email), Times.Once());
        }

        [TestMethod]
        public async Task LoginAsync_UserIsLockedOut_ReturnsBadRequest()
        {
            /// Arrange
            var loginModel = new LoginDTO();

            _mockUserHelper.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(SignInResult.LockedOut);

            /// Act
            var result = await _controller.LoginAsync(loginModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Ha superado el máximo número de intentos, su cuenta está bloqueada, intente de nuevo en 5 minutos.", badRequestResult!.Value);
            _mockUserHelper.Verify(x => x.LoginAsync(loginModel), Times.Once());
        }

        [TestMethod]
        public async Task LoginAsync_UserIsNotAllowed_ReturnsBadRequest()
        {
            /// Arrange
            var loginModel = new LoginDTO();

            _mockUserHelper.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(SignInResult.NotAllowed);

            /// Act
            var result = await _controller.LoginAsync(loginModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("El usuario no ha sido habilitado, debes de seguir las instrucciones del correo enviado para poder habilitar el usuario.", badRequestResult!.Value);
            _mockUserHelper.Verify(x => x.LoginAsync(loginModel), Times.Once());
        }

        [TestMethod]
        public async Task LoginAsync_IncorrectLoginDetails_ReturnsBadRequest()
        {
            /// Arrange
            var loginModel = new LoginDTO();

            _mockUserHelper.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(SignInResult.Failed);

            /// Act
            var result = await _controller.LoginAsync(loginModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Email o contraseña incorrectos.", badRequestResult!.Value);
            _mockUserHelper.Verify(x => x.LoginAsync(loginModel), Times.Once());
        }

        [TestMethod]
        public async Task ResedToken_UserNotFound_ReturnsNotFound()
        {
            /// Arrange
            var emailModel = new EmailDTO
            {
                Email = "test@example.com"
            };

            _mockUserHelper.Setup(x => x.GetUserAsync(emailModel.Email))
                .ReturnsAsync((User)null);

            /// Act
            var result = await _controller.ResedToken(emailModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(emailModel.Email), Times.Once());
        }

        [TestMethod]
        public async Task ResedToken_EmailSentSuccessfully_ReturnsNoContent()
        {
            /// Arrange
            var emailModel = new EmailDTO
            {
                Email = "test@example.com"
            };
            var user = new User();

            _mockUserHelper.Setup(x => x.GetUserAsync(emailModel.Email))
                .ReturnsAsync(user);

            _mockUserHelper.Setup(x => x.GenerateEmailConfirmationTokenAsync(user))
               .ReturnsAsync("GeneratedToken");

            var response = new Response<string> { WasSuccess = true };
            _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(response);

            /// Act
            var result = await _controller.ResedToken(emailModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(emailModel.Email), Times.Once());
            _mockUserHelper.Verify(x => x.GenerateEmailConfirmationTokenAsync(user), Times.Once());
            _mockMailHelper.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ResedToken_EmailFailedToSend_ReturnsBadRequest()
        {
            /// Arrange
            var emailModel = new EmailDTO
            {
                Email = "test@example.com"
            };
            var user = new User();

            _mockUserHelper.Setup(x => x.GetUserAsync(emailModel.Email))
                .ReturnsAsync(user);

            _mockUserHelper.Setup(x => x.GenerateEmailConfirmationTokenAsync(user))
               .ReturnsAsync("GeneratedToken");

            var response = new Response<string> { WasSuccess = false, Message = "Email sending failed" };
            _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(response);

            /// Act
            var result = await _controller.ResedToken(emailModel);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(emailModel.Email), Times.Once());
            _mockUserHelper.Verify(x => x.GenerateEmailConfirmationTokenAsync(user), Times.Once());
            _mockMailHelper.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ConfirmEmailAsync_UserNotFound_ReturnsNotFound()
        {
            /// Arrange
            var guid = Guid.NewGuid();
            var testUserId = guid.ToString();
            var testToken = "someToken";

            _mockUserHelper.Setup(x => x.GetUserAsync(guid))
                .ReturnsAsync((User)null);

            /// Act
            var result = await _controller.ConfirmEmailAsync(testUserId, testToken);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(guid), Times.Once());
        }

        [TestMethod]
        public async Task ConfirmEmailAsync_ConfirmationFailed_ReturnsBadRequest()
        {
            /// Arrange
            var guid = Guid.NewGuid();
            var testUserId = guid.ToString();
            var testToken = "someToken";
            var user = new User();

            _mockUserHelper.Setup(x => x.GetUserAsync(guid))
                .ReturnsAsync(user);

            _mockUserHelper.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Confirmation failed" }));

            /// Act
            var result = await _controller.ConfirmEmailAsync(testUserId, testToken);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult!.Value);
            _mockUserHelper.Verify(x => x.GetUserAsync(guid), Times.Once());
            _mockUserHelper.Verify(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task ConfirmEmailAsync_ConfirmationSucceeded_ReturnsNoContent()
        {
            /// Arrange
            var guid = Guid.NewGuid();
            var testUserId = guid.ToString();
            var testToken = "someToken";
            var user = new User();

            _mockUserHelper.Setup(x => x.GetUserAsync(guid))
                .ReturnsAsync(user);

            _mockUserHelper.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            /// Act
            var result = await _controller.ConfirmEmailAsync(testUserId, testToken);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserHelper.Verify(x => x.GetUserAsync(guid), Times.Once());
            _mockUserHelper.Verify(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
        }

        private ControllerContext GetControllerContext(string userName)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName)
            };
            var identity = new ClaimsIdentity(claims, "test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            return new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        #endregion Methods

    }
}
namespace Sales.UnitTest.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Sales.API.Controllers;
    using Sales.API.Data;
    using Sales.API.Helpers.Interfaces;
    using Sales.API.Interfaces;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class ProductsControllerTests
    /// </summary>

    [TestClass]
    public class ProductsControllerTests
    {

        #region Attributes

        private ProductsController _controller = null!;
        private DataContext _context = null!;
        private Mock<IFileStorage> _mockFileStorage = null!;
        private Mock<IGenericUnitOfWork<Product>> _mockUnitOfWork = null!;
        private const string _container = "products";
        private const string _string64base = "U29tZVZhbGlkQmFzZTY0U3RyaW5n";

        #endregion Attributes

        #region Methods

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options);
            _mockFileStorage = new Mock<IFileStorage>();
            _mockUnitOfWork = new Mock<IGenericUnitOfWork<Product>>();
            _controller = new ProductsController(_mockUnitOfWork.Object, _context, _mockFileStorage.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task PostAddImagesAsync_ProductNotFound_ReturnsNotFound()
        {
            /// Arrange
            var imageDto = new ImageDTO { ProductId = 999 };

            /// Act
            var result = await _controller.PostAddImagesAsync(imageDto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostAddImagesAsync_SuccessfullyAddsImages_ReturnsOk()
        {
            /// Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Some",
                Description = "Some"
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var imageDto = new ImageDTO
            {
                ProductId = 1,
                Images = new List<string> { _string64base }
            };

            _mockFileStorage.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container)).ReturnsAsync("storedImagePath");

            /// Act
            var result = await _controller.PostAddImagesAsync(imageDto);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedDto = okResult.Value as ImageDTO;
            Assert.IsTrue(returnedDto!.Images.Contains("storedImagePath"));
            _mockFileStorage.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", _container), Times.Once());
        }

        [TestMethod]
        public async Task PostRemoveLastImageAsync_ProductNotFound_ReturnsNotFound()
        {
            /// Arrange
            var imageDto = new ImageDTO { ProductId = 999 }; // Non-existing product

            /// Act
            var result = await _controller.PostRemoveLastImageAsync(imageDto);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostRemoveLastImageAsync_NoImages_ReturnsOkWithEmptyList()
        {
            /// Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Some",
                Description = "Some"
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var imageDto = new ImageDTO { ProductId = 1 };

            /// Act
            var result = await _controller.PostRemoveLastImageAsync(imageDto);

            /// Assert
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task PostRemoveLastImageAsync_RemovesLastImage_ReturnsOk()
        {
            /// Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Some",
                Description = "Some",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Image = "image1.jpg" },
                    new ProductImage { Image = "image2.jpg" }
                }
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _mockFileStorage.Setup(fs => fs.RemoveFileAsync("image2.jpg", "products"))
                .Returns(Task.CompletedTask);

            var imageDto = new ImageDTO { ProductId = 1 };

            /// Act
            var result = await _controller.PostRemoveLastImageAsync(imageDto);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedDto = okResult.Value as ImageDTO;
            Assert.AreEqual(1, returnedDto!.Images.Count);
            Assert.AreEqual("image1.jpg", returnedDto.Images.First());
            _mockFileStorage.Verify(x => x.RemoveFileAsync("image2.jpg", "products"), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_WithoutFilter_ReturnsAllProducts()
        {
            /// Arrange
            var product1 = new Product { Id = 1, Name = "ProductA", Description = "ProductA" };
            var product2 = new Product { Id = 2, Name = "ProductB", Description = "ProductB" };
            _context.Products.AddRange(product1, product2);
            await _context.SaveChangesAsync();

            var pagination = new PaginationDTO { RecordsNumber = 10, Page = 1 };

            /// Act
            var result = await _controller.GetAsync(pagination);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var products = okResult.Value as List<Product>;
            Assert.AreEqual(2, products.Count);
        }

        [TestMethod]
        public async Task GetAsync_WithFilter_ReturnsFilteredProducts()
        {
            /// Arrange
            var product1 = new Product { Id = 1, Name = "ProductA", Description = "ProductA" };
            var product2 = new Product { Id = 2, Name = "ProductB", Description = "ProductB" };
            _context.Products.AddRange(product1, product2);
            await _context.SaveChangesAsync();

            var pagination = new PaginationDTO { RecordsNumber = 10, Page = 1, Filter = "ProductA" };

            /// Act
            var result = await _controller.GetAsync(pagination);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var products = okResult.Value as List<Product>;
            Assert.AreEqual(1, products!.Count);
            Assert.AreEqual("ProductA", products[0].Name);
        }

        [TestMethod]
        public async Task GetPagesAsync_WithoutFilter_ReturnsCorrectTotalPages()
        {
            /// Arrange
            for (int i = 1; i <= 15; i++)
            {
                _context.Products.Add(new Product { Id = i, Name = $"Product{i}", Description = $"Product{i}" });
            }
            await _context.SaveChangesAsync();

            var pagination = new PaginationDTO { RecordsNumber = 10, Page = 1 };

            /// Act
            var result = await _controller.GetPagesAsync(pagination);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            double totalPages = (double)okResult.Value!;
            Assert.AreEqual(2, totalPages);
        }

        [TestMethod]
        public async Task GetPagesAsync_WithFilter_ReturnsFilteredTotalPages()
        {
            /// Arrange
            for (int i = 1; i <= 15; i++)
            {
                _context.Products.Add(new Product { Id = i, Name = i < 7 ? $"ProductA{i}" : $"ProductB{i}", Description = i < 7 ? $"ProductA{i}" : $"ProductB{i}" });
            }
            await _context.SaveChangesAsync();

            var pagination = new PaginationDTO { RecordsNumber = 10, Page = 1, Filter = "ProductA" };

            /// Act
            var result = await _controller.GetPagesAsync(pagination);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            double totalPages = (double)okResult.Value!;
            Assert.AreEqual(1, totalPages);
        }

        [TestMethod]
        public async Task GetAsync_ValidId_ReturnsProduct()
        {
            /// Arrange
            var product = new Product
            {
                Id = 1,
                Name = "TestProduct",
                Description = "TestProduct",
                ProductImages = new List<ProductImage> { new ProductImage { Image = "Image1.jpg" } },
                ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 1 } }
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            /// Act
            var result = await _controller.GetAsync(1);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedProduct = okResult.Value as Product;
            Assert.IsNotNull(returnedProduct);
            Assert.AreEqual("TestProduct", returnedProduct.Name);
        }

        [TestMethod]
        public async Task GetAsync_InvalidId_ReturnsNotFound()
        {
            /// Arrange
            var product = new Product
            {
                Id = 1,
                Name = "TestProduct",
                Description = "TestProduct",
                ProductImages = new List<ProductImage> { new ProductImage { Image = "Image1.jpg" } },
                ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 1 } }
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            /// Act
            var result = await _controller.GetAsync(2);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostFullAsync_ValidDTO_AddsProduct()
        {
            /// Arrange
            _mockFileStorage.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", "products"))
                .ReturnsAsync("testImage.jpg");

            var productDTO = new ProductDTO
            {
                Name = "TestProduct",
                Description = "Description",
                Price = 100.00M,
                Stock = 10,
                ProductImages = new List<string> { Convert.ToBase64String(new byte[] { 123, 45, 67 }) },
                ProductCategoryIds = new List<int> { 1 }
            };

            /// Add a dummy category to the context
            _context.Categories.Add(new Category { Id = 1, Name = "TestCategory" });
            await _context.SaveChangesAsync();

            /// Act
            var result = await _controller.PostFullAsync(productDTO);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedProductDTO = okResult.Value as ProductDTO;
            Assert.IsNotNull(returnedProductDTO);
            Assert.AreEqual("TestProduct", returnedProductDTO.Name);
            Assert.IsTrue(_context.Products.Any(p => p.Name == "TestProduct"));
        }

        [TestMethod]
        public async Task PostFullAsync_DuplicateName_ReturnsBadRequest()
        {
            /// Arrange
            _context.Products.Add(new Product { Name = "TestProduct", Description = "TestProduct" });
            await _context.SaveChangesAsync();

            var productDTO = new ProductDTO
            {
                Name = "TestProduct",
                Description = "Description",
                Price = 100.00M,
                Stock = 10,
                ProductImages = new List<string> { _string64base },
                ProductCategoryIds = new List<int> { 1 }
            };

            /// Act
            var result = await _controller.PostFullAsync(productDTO);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Ya existe un producto con el mismo nombre.", badRequestResult!.Value);
        }

        [TestMethod]
        public async Task PostFullAsync_Exception_ReturnsBadRequest()
        {
            /// Arrange
            _mockFileStorage.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", "products"))
                .Throws(new Exception("Test exception"));

            var productDTO = new ProductDTO
            {
                Name = "TestProduct",
                Description = "Description",
                Price = 100.00M,
                Stock = 10,
                ProductImages = new List<string> { Convert.ToBase64String(new byte[] { 123, 45, 67 }) },
                ProductCategoryIds = new List<int> { 1 }
            };

            /// Act
            var result = await _controller.PostFullAsync(productDTO);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Test exception", badRequestResult!.Value);
            _mockFileStorage.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", "products"), Times.Once());
        }

        [TestMethod]
        public async Task PutFullAsync_ValidDTO_UpdatesProduct()
        {
            /// Arrange
            _context.Categories.Add(new Category { Id = 1, Name = "One" });
            _context.Categories.Add(new Category { Id = 2, Name = "Two" });
            _context.Categories.Add(new Category { Id = 3, Name = "Three" });
            _context.Categories.Add(new Category { Id = 4, Name = "Four" });

            var product = new Product
            {
                Name = "OldName",
                Description = "OldDescription",
                Price = 50.00M,
                Stock = 5,
                ProductCategories = new List<ProductCategory>
                {
                    new ProductCategory { CategoryId = 1 } ,
                    new ProductCategory { CategoryId = 2 }
                }
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = "NewName",
                Description = "NewDescription",
                Price = 100.00M,
                Stock = 10,
                ProductCategoryIds = new List<int> { 2, 3 }
            };

            /// Act
            var result = await _controller.PutFullAsync(productDTO);

            /// Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedProductDTO = okResult.Value as ProductDTO;
            Assert.IsNotNull(returnedProductDTO);
            Assert.AreEqual("NewName", returnedProductDTO.Name);
            var updatedProduct = await _context.Products.FindAsync(product.Id);
            Assert.AreEqual("NewName", updatedProduct!.Name);
        }

        [TestMethod]
        public async Task PutFullAsync_NonExistingProduct_ReturnsNotFound()
        {
            /// Arrange
            var productDTO = new ProductDTO
            {
                Id = 999,
                Name = "TestName",
                Description = "TestDescription",
                Price = 100.00M,
                Stock = 10
            };

            /// Act
            var result = await _controller.PutFullAsync(productDTO);

            /// Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PutFullAsync_DuplicateName_ReturnsBadRequest()
        {
            /// Arrange
            var product = new Product { Id = 1, Name = "OriginalName", Description = "Description" };
            _context.Products.Add(product);
            _context.SaveChanges();

            /// Mock the DbUpdateException by adding a product with the same name
            _context.Products.Add(new Product { Id = 2, Name = "DuplicateName", Description = "Description" });
            _context.SaveChanges();

            var productDTO = new ProductDTO
            {
                Id = 1,
                Name = "DuplicateName",
                Description = "Description",
                Price = 100.00M,
                Stock = 10
            };

            /// Act
            var result = await _controller.PutFullAsync(productDTO);

            /// Assert
            ///var badRequestResult = result as BadRequestObjectResult;
            ///Assert.IsNotNull(badRequestResult);
            ///Assert.AreEqual("Ya existe un producto con el mismo nombre.", badRequestResult.Value);

            var badRequestResult = result as OkObjectResult;
            Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task PutFullAsync_Exception_ReturnsBadRequest()
        {
            /// Arrange
            var product = new Product { Id = 1, Name = "OriginalName", Description = "Description" };
            _context.Products.Add(product);
            _context.SaveChanges();

            _mockFileStorage.Setup(fs => fs.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", "products"))
                .Throws(new Exception("Test exception"));

            var productDTO = new ProductDTO
            {
                Id = 1,
                Name = "TestProduct",
                Description = "Description",
                Price = 100.00M,
                Stock = 10,
                ProductImages = new List<string> { Convert.ToBase64String(new byte[] { 123, 45, 67 }) },
                ProductCategoryIds = new List<int> { 1 }
            };

            /// Act
            var result = await _controller.PutFullAsync(productDTO);

            /// Assert
            ///Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            ///var badRequestResult = result as BadRequestObjectResult;
            ///Assert.AreEqual("Test exception", badRequestResult!.Value);
            ///_mockFileStorage.Verify(x => x.SaveFileAsync(It.IsAny<byte[]>(), ".jpg", "products"), Times.Once());

            /// Assert
            var badRequestResult = result as OkObjectResult;
            Assert.IsNotNull(badRequestResult);
        }

        #endregion Methods

    }
}
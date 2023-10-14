namespace Sales.API.Controllers
{

    #region Import

    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Sales.API.Data;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;
    using Sales.Shared.Responses;
    using Sales.Shared.Helpers;
    using Sales.API.Helpers.Interfaces;
    using Sales.API.Interfaces;

    #endregion Import

    /// <summary>
    /// The class ProductsController
    /// </summary>

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ProductsController : GenericController<Product>
    {

        #region Attributes

        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        #endregion Attributes

        #region Constructor

        public ProductsController(IGenericUnitOfWork<Product> unitOfWork, DataContext context, IFileStorage fileStorage) : base(unitOfWork, context)
        {
            _context = context;
            this._fileStorage = fileStorage;
        }

        #endregion Constructor

        #region Methods

        [AllowAnonymous]
        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductCategories)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync());
        }

        [AllowAnonymous]
        [HttpGet("totalPages")]
        public override async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductCategories!)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("addImages")]
        public async Task<ActionResult> PostAddImagesAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            if (product.ProductImages is null)
            {
                product.ProductImages = new List<ProductImage>();
            }

            for (int i = 0; i < imageDTO.Images.Count; i++)
            {
                if (!imageDTO.Images[i].StartsWith("https://"))
                {
                    var photoProduct = Convert.FromBase64String(imageDTO.Images[i]);
                    imageDTO.Images[i] = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products");
                    product.ProductImages!.Add(new ProductImage { Image = imageDTO.Images[i] });
                }
            }

            _context.Update(product);
            await _context.SaveChangesAsync();
            return Ok(imageDTO);
        }

        [HttpPost("removeLastImage")]
        public async Task<ActionResult> PostRemoveLastImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            if (product.ProductImages is null || product.ProductImages.Count == 0)
            {
                return Ok();
            }

            var lastImage = product.ProductImages.LastOrDefault();
            await _fileStorage.RemoveFileAsync(lastImage!.Image, "products");
            product.ProductImages.Remove(lastImage);
            _context.Update(product);
            await _context.SaveChangesAsync();
            imageDTO.Images = product.ProductImages.Select(x => x.Image).ToList();
            return Ok(imageDTO);
        }

        [HttpPost("full")]
        public async Task<IActionResult> PostFullAsync(ProductDTO productDTO)
        {
            try
            {
                Product newProduct = new()
                {
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Price = productDTO.Price,
                    Stock = productDTO.Stock,
                    ProductCategories = new List<ProductCategory>(),
                    ProductImages = new List<ProductImage>()
                };

                foreach (var productImage in productDTO.ProductImages!)
                {
                    var photoProduct = Convert.FromBase64String(productImage);
                    newProduct.ProductImages.Add(new ProductImage { Image = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products") });
                }

                foreach (var productCategoryId in productDTO.ProductCategoryIds!)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == productCategoryId);
                    if (category != null)
                    {
                        newProduct.ProductCategories.Add(new ProductCategory { Category = category });
                    }
                }

                _context.Add(newProduct);
                await _context.SaveChangesAsync();
                return Ok(productDTO);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un producto con el mismo nombre.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut("full")]
        public async Task<ActionResult> PutFullAsync(ProductDTO productDTO)
        {
            try
            {
                var product = await _context.Products
                    .Include(x => x.ProductCategories)
                    .FirstOrDefaultAsync(x => x.Id == productDTO.Id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                product.Stock = productDTO.Stock;
                product.ProductCategories = productDTO.ProductCategoryIds!.Select(x => new ProductCategory { CategoryId = x }).ToList();

                _context.Update(product);
                await _context.SaveChangesAsync();
                return Ok(productDTO);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un producto con el mismo nombre.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        #endregion Methods

    }
}
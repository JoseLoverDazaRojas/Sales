namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;
    using Sales.API.Interfaces;

    #endregion Import

    /// <summary>
    /// The class TemporalOrdersController
    /// </summary>

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class TemporalOrdersController : GenericController<TemporalOrder>
    {

        #region Attributes

        private readonly DataContext _context;

        #endregion Attributes

        #region Constructor

        public TemporalOrdersController(IGenericUnitOfWork<TemporalOrder> unitOfWork, DataContext context) : base(unitOfWork, context)
        {
            _context = context;
        }

        #endregion Constructor

        #region Methods

        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            return Ok(await _context.TemporalOrders
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.TemporalOrders
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .Where(x => x.User!.Email == User.Identity!.Name)
                .ToListAsync());
        }

        [HttpGet("count")]
        public async Task<ActionResult> GetCountAsync()
        {
            return Ok(await _context.TemporalOrders
                .Where(x => x.User!.Email == User.Identity!.Name)
                .SumAsync(x => x.Quantity));
        }

        [HttpPost("full")]
        public async Task<IActionResult> PostAsync(TemporalOrderDTO temporalOrderDTO)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalOrderDTO.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity!.Name);
            if (user == null)
            {
                return NotFound();
            }

            var temporalOrder = new TemporalOrder
            {
                Product = product,
                Quantity = temporalOrderDTO.Quantity,
                Remarks = temporalOrderDTO.Remarks,
                User = user
            };

            try
            {
                _context.Add(temporalOrder);
                await _context.SaveChangesAsync();
                return Ok(temporalOrderDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("full")]
        public async Task<IActionResult> PutAsync(TemporalOrderDTO temporalOrderDTO)
        {
            var currentTemporalOrder = await _context.TemporalOrders.FirstOrDefaultAsync(x => x.Id == temporalOrderDTO.Id);
            if (currentTemporalOrder == null)
            {
                return NotFound();
            }

            currentTemporalOrder!.Remarks = temporalOrderDTO.Remarks;
            currentTemporalOrder.Quantity = temporalOrderDTO.Quantity;

            _context.Update(currentTemporalOrder);
            await _context.SaveChangesAsync();
            return Ok(temporalOrderDTO);
        }

        #endregion Methods

    }
}
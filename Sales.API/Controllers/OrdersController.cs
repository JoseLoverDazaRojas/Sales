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
    using Sales.Shared.Enums;

    #endregion Import

    /// <summary>
    /// The class OrdersController
    /// </summary>

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {

        #region Attributes

        private readonly IOrdersHelper _ordersHelper;
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        #endregion Attributes

        #region Constructor

        public OrdersController(IOrdersHelper ordersHelper, DataContext context, IUserHelper userHelper)
        {
            _ordersHelper = ordersHelper;
            _context = context;
            _userHelper = userHelper;
        }

        #endregion Constructor

        #region Methods

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var sale = await _context.Orders
                .Include(s => s.User!)
                .ThenInclude(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .Include(s => s.OrderDetails!)
                .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity!.Name);
            if (user == null)
            {
                return BadRequest("User not valid.");
            }

            var queryable = _context.Orders
                .Include(s => s.User)
                .Include(s => s.OrderDetails)
                .ThenInclude(sd => sd.Product)
                .AsQueryable();

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == User.Identity!.Name);
            }

            return Ok(await queryable
                .OrderByDescending(x => x.Date)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity!.Name);
            if (user == null)
            {
                return BadRequest("User not valid.");
            }

            var queryable = _context.Orders
                .AsQueryable();

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == User.Identity!.Name);
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(OrderDTO saleDTO)
        {
            var response = await _ordersHelper.ProcessOrderAsync(User.Identity!.Name!, saleDTO.Remarks);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(OrderDTO orderDTO)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin && orderDTO.OrderStatus != OrderStatus.Cancelled)
            {
                return BadRequest("Solo permitido para administradores.");
            }

            var order = await _context.Orders
                .Include(s => s.OrderDetails)
                .FirstOrDefaultAsync(s => s.Id == orderDTO.Id);
            if (order == null)
            {
                return NotFound();
            }

            if (orderDTO.OrderStatus == OrderStatus.Cancelled)
            {
                await ReturnStockAsync(order);
            }

            order.OrderStatus = orderDTO.OrderStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        private async Task ReturnStockAsync(Order sale)
        {
            foreach (var saleDetail in sale.OrderDetails!)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == saleDetail.ProductId);
                if (product != null)
                {
                    product.Stock += saleDetail.Quantity;
                }
            }
            await _context.SaveChangesAsync();
        }

        #endregion Methods

    }
}
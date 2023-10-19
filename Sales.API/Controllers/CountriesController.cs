namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;
    using Sales.API.Interfaces;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;
    using Sales.Shared.Helpers;

    #endregion Import

    /// <summary>
    /// The class CountriesController
    /// </summary>

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class CountriesController : GenericController<Country>
    {

        #region Attributes

        private readonly IGenericUnitOfWork<Country> _unitOfWork;
        private readonly DataContext _context;

        #endregion Attributes

        #region Constructor

        public CountriesController(IGenericUnitOfWork<Country> unitOfWork, DataContext context) : base(unitOfWork, context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #endregion Constructor

        #region Methods

        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<ActionResult> GetComboAsync()
        {
            return Ok(await _context.Countries
                .OrderBy(c => c.Name)
                .ToListAsync());
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Countries
                .Include(c => c.States)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable
                .OrderBy(c => c.Name)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public override async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Countries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var country = await _unitOfWork.GetCountryAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);
        }

        #endregion Methods

    }
}
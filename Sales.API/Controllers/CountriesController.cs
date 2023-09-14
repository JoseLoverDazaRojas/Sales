namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;
    using Sales.API.Helpers;
    using Sales.API.Interfaces;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class CountriesController
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : GenericController<Country>
    {

        #region Attributes

        private readonly DataContext _context;

        #endregion Attributes

        #region Constructor

        public CountriesController(IGenericUnitOfWork<Country> unitOfWork, DataContext context) : base(unitOfWork, context)
        {
            _context = context;
        }

        #endregion Constructor

        #region Methods

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Countries
                .Include(c => c.States)
                .AsQueryable();

            return Ok(await queryable
                .OrderBy(c => c.Name)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var country = await _context.Countries
                .Include(c => c.States!)
                .ThenInclude(s => s.Cities)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);
        }

        #endregion Methods

    }
}
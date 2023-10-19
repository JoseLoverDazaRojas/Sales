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
    /// The class CitiesController
    /// </summary>

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class CitiesController : GenericController<City>
    {

        #region Attributes

        private readonly DataContext _context;

        #endregion Attributes

        #region Constructor

        public CitiesController(IGenericUnitOfWork<City> unitOfWork, DataContext context) : base(unitOfWork, context)
        {
            _context = context;
        }

        #endregion Constructor

        #region Methods

        [AllowAnonymous]
        [HttpGet("combo/{stateId:int}")]
        public async Task<ActionResult> GetComboAsync(int stateId)
        {
            return Ok(await _context.Cities
                .Where(c => c.StateId == stateId)
                .OrderBy(c => c.Name)
                .ToListAsync());
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Cities
                .Where(x => x.State!.Id == pagination.Id)
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

        [HttpGet("totalPages")]
        public override async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Cities
                .Where(x => x.State!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        #endregion Methods
    }
}
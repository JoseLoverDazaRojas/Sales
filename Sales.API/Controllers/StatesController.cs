namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;
    using Sales.API.Intertfaces;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class StatesController
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class StatesController : GenericController<State>
    {

        #region Attributes

        private readonly DataContext _context;

        #endregion Attributes

        #region Constructor

        public StatesController(IGenericUnitOfWork<State> unitOfWork, DataContext context) : base(unitOfWork)
        {
            _context = context;
        }

        #endregion Constructor

        #region Methods

        [HttpGet]
        public override async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.States
                .Include(s => s.Cities)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var state = await _context.States
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }
            return Ok(state);
        }

        #endregion Methods

    }
}
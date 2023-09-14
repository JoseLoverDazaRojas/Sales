namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;
    using Sales.API.Helpers;
    using Sales.API.Interfaces;
    using Sales.Shared.DTOs;

    #endregion Import

    public class GenericController<T> : Controller where T : class
    {

        #region Attributes

        private readonly IGenericUnitOfWork<T> _unitOfWork;
        private readonly DataContext _context;
        private readonly DbSet<T> _entity;

        #endregion Attributes

        #region Constructor

        public GenericController(IGenericUnitOfWork<T> unitOfWork, DataContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _entity = context.Set<T>();
        }

        #endregion Constructor

        #region Methods

        [HttpGet]
        public virtual async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _entity.AsQueryable();
            return Ok(await queryable
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public virtual async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _entity.AsQueryable();
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }


        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetAsync(int id)
        {
            var action = await _unitOfWork.GetAsync(id);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound();
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostAsync(T model)
        {
            var action = await _unitOfWork.AddAsync(model);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpPut]
        public virtual async Task<IActionResult> PutAsync(T model)
        {
            var action = await _unitOfWork.UpdateAsync(model);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(int id)
        {
            var action = await _unitOfWork.GetAsync(id);
            if (!action.WasSuccess)
            {
                return NotFound();
            }
            action = await _unitOfWork.DeleteAsync(id);
            if (!action.WasSuccess)
            {
                return BadRequest(action.Message);
            }
            return NoContent();
        }

        #endregion Methods

    }
}
namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Sales.API.Data;
    using Sales.API.Interfaces;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class CategoriesController
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : GenericController<Category>
    {

        #region Attributes

        private readonly DataContext _context;

        #endregion Attributes

        #region Constructor

        public CategoriesController(IGenericUnitOfWork<Category> unitOfWork, DataContext context) : base(unitOfWork, context)
        {
        }

        #endregion Constructor

        #region Methods

        #endregion Methods

    }
}
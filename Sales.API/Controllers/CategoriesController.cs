namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Sales.API.Intertfaces;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class CategoriesController
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : GenericController<Category>
    {

        #region Methods

        public CategoriesController(IGenericUnitOfWork<Category> unitOfWork) : base(unitOfWork)
        {
        }

        #endregion Methods

    }
}
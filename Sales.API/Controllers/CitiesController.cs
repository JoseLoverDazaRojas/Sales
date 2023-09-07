namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Sales.API.Intertfaces;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class CitiesController
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : GenericController<City>
    {
        #region Methods

        public CitiesController(IGenericUnitOfWork<City> unitOfWork) : base(unitOfWork)
        {
        }

        #endregion Methods
    }
}
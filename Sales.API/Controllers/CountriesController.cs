namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
    using Sales.API.Intertfaces;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class CountriesController
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : GenericController<Country>
    {

        #region Methods

        public CountriesController(IGenericUnitOfWork<Country> unitOfWork) : base(unitOfWork)
        {
        }

        #endregion Methods

    }
}
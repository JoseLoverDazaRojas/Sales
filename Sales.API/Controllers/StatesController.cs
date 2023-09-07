namespace Sales.API.Controllers
{

    #region Import

    using Microsoft.AspNetCore.Mvc;
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

        #region Methods

        public StatesController(IGenericUnitOfWork<State> unitOfWork) : base(unitOfWork)
        {
        }

        #endregion Methods

    }
}
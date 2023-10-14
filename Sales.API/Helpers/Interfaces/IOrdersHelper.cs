namespace Sales.API.Helpers.Interfaces
{

    #region Import

    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The interface IOrdersHelper
    /// </summary>

    public interface IOrdersHelper
    {

        #region Methods

        public Task<Response<bool>> ProcessOrderAsync(string email, string remarks);

        #endregion Methods

    }
}
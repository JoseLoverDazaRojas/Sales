namespace Sales.API.Services
{

    #region Import

    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The interface IApiService
    /// </summary>

    public interface IApiService
    {

        #region Methods

        public Task<Response<T>> GetAsync<T>(string servicePrefix, string controller);

        #endregion Methods

    }
}
namespace Sales.WEB.Repositories
{

    /// <summary>
    /// The interface IRepository
    /// </summary>

    public interface IRepository
    {

        #region Methods

        Task<HttpResponseWrapper<T>> Get<T>(string url);
        Task<HttpResponseWrapper<object>> Post<T>(string url, T model);
        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T model);

        #endregion Methods
    }
}

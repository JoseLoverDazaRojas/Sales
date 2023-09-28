namespace Sales.WEB.Repositories
{

    /// <summary>
    /// The interface IRepository
    /// </summary>

    public interface IRepository
    {

        #region Methods

        public Task<HttpResponseWrapper<T>> GetAsync<T>(string url);

        public Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model);

        public Task<HttpResponseWrapper<TResponse>> PostAsync<T, TResponse>(string url, T model);

        public Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model);

        public Task<HttpResponseWrapper<TResponse>> PutAsync<T, TResponse>(string url, T model);

        public Task<HttpResponseWrapper<object>> DeleteAsync(string url);
        
        #endregion Methods
    }
}
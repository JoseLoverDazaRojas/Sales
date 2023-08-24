namespace Sales.WEB.Repositories
{
    using System.Text;
    using System.Text.Json;

    public class Repository : IRepository
    {
        private readonly HttpClient _httpClient;

        public Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseWrapper<object>> Post<T>(string url, T model)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T model)
        {
            throw new NotImplementedException();
        }
    }
}

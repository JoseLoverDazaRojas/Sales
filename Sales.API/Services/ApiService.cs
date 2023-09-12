namespace Sales.API.Services
{

    #region Import

    using System.Text.Json;
    using Sales.Shared.Responses;

    #endregion Import


    /// <summary>
    /// The class ApiService
    /// </summary>

    public class ApiService : IApiService 
    {

        #region Attributes

        private readonly string _urlBase;
        private readonly string _tokenName;
        private readonly string _tokenValue;

        #endregion Attributes

        #region Constructor

        public ApiService(IConfiguration configuration)
        {
            _urlBase = configuration["CoutriesAPI:urlBase"]!;
            _tokenName = configuration["CoutriesAPI:tokenName"]!;
            _tokenValue = configuration["CoutriesAPI:tokenValue"]!;
        }

        #endregion Constructor

        #region Methods

        private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<Response<T>> GetAsync<T>(string servicePrefix, string controller)
        {
            try
            {
                var client = new HttpClient()
                {
                    BaseAddress = new Uri(_urlBase),
                };

                client.DefaultRequestHeaders.Add(_tokenName, _tokenValue);
                var url = $"{servicePrefix}{controller}";
                var responseHttp = await client.GetAsync(url);
                var response = await responseHttp.Content.ReadAsStringAsync();
                if (!responseHttp.IsSuccessStatusCode)
                {
                    return new Response<T>
                    {
                        WasSuccess = false,
                        Message = response
                    };
                }

                return new Response<T>
                {
                    WasSuccess = true,
                    Result = JsonSerializer.Deserialize<T>(response, _jsonDefaultOptions)!
                };
            }
            catch (Exception ex)
            {
                return new Response<T>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        #endregion Methods

    }
}
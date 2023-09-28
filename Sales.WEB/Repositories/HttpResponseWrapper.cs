namespace Sales.WEB.Repositories
{

    #region Import

    using System.Net;

    #endregion Import

    /// <summary>
    /// The class HttpResponseWrapper<T>
    /// </summary>

    public class HttpResponseWrapper<T>
    {
        #region Attributes

        public bool Error { get; set; }

        public T? Response { get; set; }

        public HttpResponseMessage HttpResponseMessage { get; set; }

        #endregion Attributes

        #region Constructor

        public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage)
        {
            Error = error;
            Response = response;
            HttpResponseMessage = httpResponseMessage;
        }

        #endregion Constructor

        #region Methods

        public async Task<string?> GetErrorMessageAsync()
        {
            if (!Error)
            {
                return null;
            }

            var statusCode = HttpResponseMessage.StatusCode;

            if (statusCode == HttpStatusCode.NotFound)
            {
                return "Recurso no encontrado";
            }

            if (statusCode == HttpStatusCode.BadRequest)
            {
                return await HttpResponseMessage.Content.ReadAsStringAsync();
            }

            if (statusCode == HttpStatusCode.Unauthorized)
            {
                return "Tienes que logearte para hacer esta operación";
            }

            if (statusCode == HttpStatusCode.Forbidden)
            {
                return "No tienes permisos para hacer esta operación";
            }

            return "Ha ocurrido un error inesperado";
        }

        #endregion Methods

    }
}
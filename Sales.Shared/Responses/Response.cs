namespace Sales.Shared.Responses
{

    /// <summary>
    /// The class Response
    /// </summary>

    public class Response<T>
    {

        #region Attributes

        public bool WasSuccess { get; set; }

        public string? Message { get; set; }

        public T? Result { get; set; }

        #endregion Attributes

    }
}
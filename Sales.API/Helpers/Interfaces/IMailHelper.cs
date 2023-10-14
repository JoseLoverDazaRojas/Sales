namespace Sales.API.Helpers.Interfaces
{

    #region Import

    using Sales.Shared.Responses;

    #endregion Import

    /// <summary>
    /// The interface IMailHelper
    /// </summary>

    public interface IMailHelper
    {

        #region Methods

        public Response<string> SendMail(string toName, string toEmail, string subject, string body);

        #endregion Methods

    }
}
namespace Sales.WEB.Interfaces
{

    /// <summary>
    /// The interface ILoginService
    /// </summary>

    public interface ILoginService
    {

        #region Methods

        public Task LoginAsync(string token);

        public Task LogoutAsync();

        #endregion Methods

    }
}
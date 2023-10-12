namespace Sales.WEB.Auth
{

    #region Import

    using System.Security.Claims;
    using Microsoft.AspNetCore.Components.Authorization;

    #endregion Import

    /// <summary>
    /// The class AuthenticationProviderTest
    /// </summary>

    public class AuthenticationProviderTest : AuthenticationStateProvider
    {

        #region Methods

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay(500);

            var anonimous = new ClaimsIdentity();

            var adminUser = new ClaimsIdentity(new List<Claim>
            {
                new Claim("FirstName", "Jose Lover"),
                new Claim("LastName", "Daza Rojas"),
                new Claim(ClaimTypes.Name, "josel8485@gmail.com"),
                new Claim(ClaimTypes.Role, "Admin")
            },
            authenticationType: "test");

            var otherUser = new ClaimsIdentity(new List<Claim>
            {
                new Claim("FirstName", "Jose "),
                new Claim("LastName", "Daza"),
                new Claim(ClaimTypes.Name, "josel8485@hotmail.com"),
                new Claim(ClaimTypes.Role, "User")
            },
            authenticationType: "test");

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(adminUser)));
        }

        #endregion Methods

    }
}
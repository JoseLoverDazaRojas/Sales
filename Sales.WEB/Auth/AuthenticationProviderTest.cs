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
                new Claim("FirstName", "Juan"),
                new Claim("LastName", "Zulu"),
                new Claim(ClaimTypes.Name, "zulu@yopmail.com"),
                new Claim(ClaimTypes.Role, "Admin")
            },
            authenticationType: "test");

            var otherUser = new ClaimsIdentity(new List<Claim>
            {
                new Claim("FirstName", "Ledys"),
                new Claim("LastName", "Bedoya"),
                new Claim(ClaimTypes.Name, "ledys@yopmail.com"),
                new Claim(ClaimTypes.Role, "User")
            },
            authenticationType: "test");

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(adminUser)));
        }

        #endregion Methods

    }
}
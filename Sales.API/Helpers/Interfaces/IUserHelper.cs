namespace Sales.API.Helpers.Interfaces
{

    #region Import

    using Microsoft.AspNetCore.Identity;
    using Sales.Shared.Entities;

    #endregion Import


    /// <summary>
    /// The interface IUserHelper
    /// </summary>

    public interface IUserHelper
    {

        #region Methods

        Task<User> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        #endregion Methods

    }
}
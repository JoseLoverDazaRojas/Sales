namespace Sales.API.Helpers.Interfaces
{

    #region Import

    using Microsoft.AspNetCore.Identity;
    using Sales.Shared.DTOs;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The interface IUserHelper
    /// </summary>

    public interface IUserHelper
    {

        #region Methods

        public Task<User> GetUserAsync(string email);

        public Task<IdentityResult> AddUserAsync(User user, string password);

        public Task CheckRoleAsync(string roleName);

        public Task AddUserToRoleAsync(User user, string roleName);

        public Task<bool> IsUserInRoleAsync(User user, string roleName);

        public Task<SignInResult> LoginAsync(LoginDTO model);

        public Task LogoutAsync();

        public Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

        public Task<IdentityResult> UpdateUserAsync(User user);

        public Task<User> GetUserAsync(Guid userId);

        public Task<string> GenerateEmailConfirmationTokenAsync(User user);

        public Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        public Task<string> GeneratePasswordResetTokenAsync(User user);

        public Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        #endregion Methods

    }
}
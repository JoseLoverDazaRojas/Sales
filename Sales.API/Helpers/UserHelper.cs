﻿namespace Sales.API.Helpers
{

    #region Import

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Sales.API.Data;
    using Sales.API.Helpers.Interfaces;
    using Sales.Shared.Entities;

    #endregion Import

    /// <summary>
    /// The class UserHelper
    /// </summary>

    public class UserHelper : IUserHelper
    {

        #region Attributes

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        #endregion Attributes

        #region Constructor

        public UserHelper(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        #endregion Constructor

        #region Methods

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(x => x.Email == email);
            return user!;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        #endregion Methods

    }
}
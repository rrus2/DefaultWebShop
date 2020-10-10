using AspNetCore;
using DefaultWebShop.Migrations;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using DefaultWebShopTests.Fixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.AdminTests
{
    public class AdminServiceTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ServiceProvider _provider;
        private readonly ApplicationDbContext _context;
        private readonly AdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminServiceTests()
        {
            _provider = new DbFixture().Provider;

            _context = _provider.GetService<ApplicationDbContext>();
            _userManager = _provider.GetService<UserManager<ApplicationUser>>();
            _roleManager = _provider.GetService<RoleManager<IdentityRole>>();
            _adminService = new AdminService(_userManager, _context, _roleManager);

            SeedRoles();
        }

        [Fact]
        public async void CreateUserWorks()
        {
            var user = new UserViewModel
            {
                Email = "pavel@hotmail.com",
                Role = "Admin",
                Birthdate = Convert.ToDateTime("30/07/1991"),
                Password = "testT_12345!",
                RepeatPassword = "testT_12345!"
            };

            var createdUser = await _adminService.CreateUser(user);

            Assert.Equal(createdUser.UserName, user.Email);
            Assert.NotNull(createdUser);
        }

        [Theory]
        [InlineData("", "Admin", "30/07/1991", "testT_12345!", "testT_12345!")]
        [InlineData(null, "Admin", "30/07/1991", "testT_12345!", "testT_12345!")]
        [InlineData("pavel@hotmail.com", "", "30/07/1991", "testT_12345!", "testT_12345!")]
        [InlineData("pavel@hotmail.com", null, "30/07/1991", "testT_12345!", "testT_12345!")]
        [InlineData("pavel@hotmail.com", "Admin", "30/07/2050", "testT_12345!", "testT_12345!")]
        [InlineData("pavel@hotmail.com", "Admin", "30/07/1991", "", "testT_12345!")]
        [InlineData("pavel@hotmail.com", "Admin", "30/07/1991", null, "testT_12345!")]
        [InlineData("pavel@hotmail.com", "Admin", "30/07/1991", "abc", "testT_12345!")]
        [InlineData("pavel@hotmail.com", "Admin", "30/07/1991", "testT_12345!", "")]
        [InlineData("pavel@hotmail.com", "Admin", "30/07/1991", "testT_12345!", null)]
        [InlineData("pavel@hotmail.com", "Admin", "30/07/1991", "testT_12345!", "abc")]
        [InlineData("", "", "30/07/1991", "", "")]
        [InlineData(null, null, null, null, null)]
        public async void CreateUserFails(string name, string role, string date, string password, string repeatpassword)
        {
            var user = new UserViewModel
            {
                Email = name,
                Birthdate = Convert.ToDateTime(date),
                Role = role,
                Password = password,
                RepeatPassword = repeatpassword
            };

            await Assert.ThrowsAnyAsync<Exception>(() => _adminService.CreateUser(user));
        }
        private async void SeedRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
        }
        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}

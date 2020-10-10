using DefaultWebShop.Controllers;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using DefaultWebShopTests.Fixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.AdminTests
{
    public class AdminServiceControllerTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private readonly GenreService _genreService;
        private readonly ProductService _productService;
        private readonly AdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ServiceProvider _provider;
        private AdminController _controller;

        public AdminServiceControllerTests()
        {
            _provider = new DbFixture().Provider;

            _context = _provider.GetService<ApplicationDbContext>();
            _genreService = new GenreService(_context);
            _productService = new ProductService(_context, null);
            _userManager = _provider.GetService<UserManager<ApplicationUser>>();
            _roleManager = _provider.GetService<RoleManager<IdentityRole>>();
            _adminService = new AdminService(_userManager, _context, _roleManager);

            _controller = new AdminController(_genreService, _productService, _roleManager, _adminService);

            SeedUser();
        }     

        [Fact]
        public async void CreateUserWorks()
        {
            var userViewModel = new UserViewModel
            {
                Email = "test@test.com",
                Birthdate = Convert.ToDateTime("30/07/1991"),
                Password = "testT_12345!",
                RepeatPassword = "testT_12345!",
                Role = "User"
            };

            var result = await _controller.CreateUser(userViewModel) as ViewResult;
            Assert.Equal("Index", result.ViewName);
        }
        [Theory]
        [InlineData("", "30/07/1991", "testT_12345!", "testT_12345!", "User")]
        [InlineData("test@test.com", "", "testT_12345!", "testT_12345!", "User")]
        [InlineData("test@test.com", "30/07/1991", "", "testT_12345!", "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", "", "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", "testT_12345!", "TEST")]
        [InlineData(null, "30/07/1991", "testT_12345!", "testT_12345!", "User")]
        [InlineData("test@test.com", "30/07/1991", null, "testT_12345!", "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", null, "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", "testT_12345!", null)]
        [InlineData("", "", "", "", "")]
        [InlineData(null, null, null, null, null)]
        public async void CreateUserFailsWithBadData(string name, string date, string password, string repeatpassword, string role)
        {
            var userViewModel = new UserViewModel
            {
                Email = name,
                Birthdate = Convert.ToDateTime(date),
                Password = password,
                RepeatPassword = repeatpassword,
                Role = role
            };

            await Assert.ThrowsAnyAsync<Exception>(() => _controller.CreateUser(userViewModel));
        }
        [Fact]
        public async void CreateUserWithFailModelFails()
        {
            var userVM = new UserViewModel();
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateUser(userVM));
        }
        private async void SeedUser()
        {
            var user = new ApplicationUser
            {
                Email = "pavel@hotmail.com",
                UserName = "pavel@hotmail.com",
                Birthdate = Convert.ToDateTime("30/07/1991")
            };

            var r = await _userManager.CreateAsync(user, "tesT1234567_!");

            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "User" });

            await _userManager.AddToRoleAsync(user, "Admin");
            await _userManager.AddToRoleAsync(user, "User");

            var rolesStr = new string[2] { "Admin", "User" };

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, "pavel@hotmail.com", ClaimValueTypes.String, "pavel@hotmail.com"),
            };

            var identity = new GenericPrincipal(new ClaimsIdentity(claims), rolesStr);

            _controller.ControllerContext = new ControllerContext(new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            });

            _controller.HttpContext.User = identity;
        }
        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}

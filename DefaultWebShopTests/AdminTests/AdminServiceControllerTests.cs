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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async void CreateUserWorksAsAdmin()
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
        [InlineData("test@test.com", "30/07/1991", "", "testT_12345!", "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", "", "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", "testT_12345!", "TEST")]
        [InlineData(null, "30/07/1991", "testT_12345!", "testT_12345!", "User")]
        [InlineData("test@test.com", "30/07/1991", null, "testT_12345!", "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", null, "User")]
        [InlineData("test@test.com", "30/07/1991", "testT_12345!", "testT_12345!", null)]
        [InlineData("", "30/07/1991", "", "", "")]
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

        [Fact]
        public async void CreateProductAsAdminWorks()
        {
            var productVM = new ProductViewModel
            {
                Name = "Something",
                Price = 50,
                Stock = 5,
                GenreID = 1,
                Amount = 5
            };

            var result = await _controller.CreateProduct(productVM, null);

            var product = _context.Products.FirstOrDefault(x => x.Name == productVM.Name);

            Assert.NotNull(product);
            Assert.Equal(productVM.Name, product.Name);
        }
        [Theory]
        [InlineData("", 50, 5, 1, 5)]
        [InlineData("ABC", 0, 5, 1, 5)]
        [InlineData("ABC", 50, 0, 1, 5)]
        [InlineData("ABC", 50, 1, 0, 5)]
        [InlineData("ABC", 50, 1, 1, 0)]
        [InlineData(null, 50,1,1,5)]
        [InlineData("ABC", -1, 1, 1, 5)]
        [InlineData("ABC", 50, -1, 1, 5)]
        [InlineData("ABC", 50, 1, -1 , 5)]
        [InlineData("ABC", 50, 1, 1, -1)]
        [InlineData(null, 0, 0, 0, 0)]
        public async void CreateProductAsAdminFailsWithBadData(string name, int price, int stock, int genreID, int amount)
        {
            var productVM = new ProductViewModel
            {
                Name = name,
                Price = price,
                Stock = stock,
                GenreID = genreID,
                Amount = amount
            };

            await Assert.ThrowsAsync<Exception>(() => _controller.CreateProduct(productVM, null));
        }

        [Fact]
        public async void CreateProductAsAdminFailsWithFailModel()
        {
            var productVM = new ProductViewModel();

            await Assert.ThrowsAsync<Exception>(() => _controller.CreateProduct(productVM, null));
        }

        [Fact]
        public async void CreateRoleAsAdminWorks()
        {
            var role = await _adminService.CreateRole(new IdentityRoleViewModel { Name = "NewRole" });

            Assert.NotNull(role);
            Assert.Equal("NewRole", role.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async void CreateRoleAsAdminFailsWithBadData(string role)
        {
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateRole(new IdentityRoleViewModel { Name = role }));
        }
        [Fact]
        public async void CreateRoleAsAdminFailsWithFailModel()
        {
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateRole(new IdentityRoleViewModel()));
        }

        [Fact]
        public async void LoadEditUserPageWorks()
        {
            var result = await _controller.EditUser() as ViewResult;
            var viewData = result.ViewData;
            var list = viewData;

            Assert.NotNull(viewData);
        }

        [Fact]
        public async void EditUserWorksAsAdmin()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var userVM = new UserViewModel
            {
                Email = "test@hotmail.com",
                Birthdate = Convert.ToDateTime("30/08/1991"),
                Id = user.Id,
                Password = "testT_12345!",
                RepeatPassword = "testT_12345!",
                Role = "User"
            };

            var updatedUser = await _adminService.UpdateUser(userVM);
            var roles = await _userManager.GetRolesAsync(updatedUser);

            Assert.Equal("test@hotmail.com", updatedUser.UserName);
            Assert.Contains("User", roles);
        }
        [Theory]
        [InlineData("", "30/07/1991", "Admin", "testT_12345_!", "testT_12345_!")]
        [InlineData("test@hotmail.com", "30/07/1991", "", "testT_12345_!", "testT_12345_!")]
        [InlineData("test@hotmail.com", "30/07/1991", "Admin", "", "testT_12345_!")]
        [InlineData("test@hotmail.com", "30/07/1991", "Admin", "testT_12345_!", "")]
        [InlineData(null, "30/07/1991", "Admin", "testT_12345_!", "testT_12345_!")]
        [InlineData("test@hotmail.com", "30/07/1991", null, "testT_12345_!", "testT_12345_!")]
        [InlineData("test@hotmail.com", "30/07/1991", "Admin", null, "testT_12345_!")]
        [InlineData("test@hotmail.com", "30/07/1991", "Admin", "testT_12345!", null)]
        [InlineData(null, "30/07/1991", null, null, null)]
        [InlineData("", "30/08/1991", "", "", "")]

        public async void EditUserFailsAsAdmin(string name, string date, string role, string password, string repeatpassword)
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var userVM = new UserViewModel
            {
                Email = name,
                Birthdate = Convert.ToDateTime(date),
                Role = role,
                Password = password,
                RepeatPassword = repeatpassword
            };

            await Assert.ThrowsAsync<Exception>(() => _adminService.UpdateUser(userVM));
        }

        [Fact]
        public async void EditUserFailsWithFailModel()
        {
            var userVM = new UserViewModel();
            await Assert.ThrowsAsync<Exception>(() => _adminService.UpdateUser(userVM));
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

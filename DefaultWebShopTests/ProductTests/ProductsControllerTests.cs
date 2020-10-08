using DefaultWebShop.Controllers;
using DefaultWebShop.Migrations;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using DefaultWebShopTests.Fixture;
using DefaultWebShopTests.ProductTests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DefaultWebShopTests.GenreTests
{
    public class ProductsControllerTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private ProductService _productService;
        private GenreService _genreService;
        private OrderService _orderService;
        private ProductsController _controller;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ServiceProvider _provider;

        public ProductsControllerTests(DbFixture fixture)
        {
            _provider = fixture.Provider;

            _context = _provider.GetService<ApplicationDbContext>();
            _context.Database.EnsureCreated();

            _productService = new ProductService(_context, null);
            _genreService = new GenreService(_context);
            _orderService = new OrderService(_context);

            _roleManager = _provider.GetService<RoleManager<IdentityRole>>();
            _userManager = _provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = _provider.GetService<SignInManager<ApplicationUser>>();

            _controller = new ProductsController(_productService, _genreService, _userManager, _orderService);

            if (_context.Genres.Count() == 0)
                SeedGenres();
            if (_context.Products.Count() == 0)
                SeedProducts();
            if (_context.Roles.Count() == 0)
                SeedRoles();
            if (_context.Users.Count() == 0)
                SeedUser();
        }
        [Fact]
        public async void IndexReturnsListOfProducts()
        {
            var result = await _controller.Index(1, 3) as ViewResult;
            var productsCount = await _productService.GetCount();
            var products = await _productService.GetProducts(1, 3);

            var model = result.Model as ProductPageViewModel;

            Assert.Equal(model.Count, productsCount);
            Assert.Equal(model.Products, products);
            Assert.Equal(model.Products.First().Name, products.First().Name);
            Assert.Equal(model.Products.Last().Name, products.Last().Name);
        }

        [Fact]
        public async void GetProductDetails()
        {
            var result = await _controller.Details(1) as ViewResult;
            var product = await _productService.GetProduct(1);
            var model = result.Model as ProductViewModel;

            Assert.Equal(model.Name, product.Name);
        }

        [Fact]
        public async void PostProductDetails()
        {
            var result = await _controller.Details(1, 2) as ViewResult;
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            Assert.NotNull(user);

        }
        [Fact]
        public async void CreateProductTestGetPageWorks()
        {
            var result = await _controller.Create() as ViewResult;
            var genres = (SelectList)result.ViewData["Genres"];
            var genresCount = genres.Count();

            Assert.Equal(3, genresCount);
        }
        [Fact]
        public async void CreateProductTestPostPageWorks()
        {
            var productviewmodel = new ProductViewModel
            {
                Name = "Nike",
                Price = 50,
                Amount = 50,
                Stock = 5,
                GenreID = 1
            };
            var result = await _controller.Create(productviewmodel, null);
            var product = await _productService.GetProductsByName(productviewmodel.Name);

            Assert.NotNull(product);
            Assert.Equal(productviewmodel.Name, product.First().Name);
        }
        [Fact]
        public async void CreateProductTestPageFailsWithBadModel()
        {
            var productviewmodel = new ProductViewModel();
            await Assert.ThrowsAsync<Exception>(() => _controller.Create(productviewmodel, null));
        }
        [Theory]
        [InlineData("", 50, 50, 5, 1)]
        [InlineData("Nike", 0, 50, 5, 1)]
        [InlineData("Nike", 50, 0, 5, 1)]
        [InlineData("Nike", 50, 50, 0, 1)]
        [InlineData("Nike", 50, 50, 5, 0)]
        [InlineData("", 0, 0, 0, 0)]
        [InlineData(null, -1, -1, -1, -1)]
        public async void CreateProductTestFailsWithBadModelData(string name, int price, int amount, int stock, int genreid)
        {
            var productVM = new ProductViewModel
            {
                Name = name,
                Price = price,
                Amount = amount,
                Stock = stock,
                GenreID = genreid
            };
            await Assert.ThrowsAsync<Exception>(() => _controller.Create(productVM, null));
        }

        [Fact]
        public async void GetProductsByGenreWorks()
        {
            var result = await _controller.ProductsByGenre(1, 1, 3) as ViewResult;
            var productsCount = await _productService.GetCountByGenreID(1);
            var products = await _productService.GetProductsByGenre(1, 3, 1);
            var model = result.Model as ProductPageViewModel;

            Assert.Equal(productsCount, model.Count);
            Assert.Equal(model.Products, products);
            Assert.Equal(model.Products.Count(), products.Count());
            Assert.Equal(model.Products.First().Name, products.First().Name);
            Assert.Equal(model.Products.Last().Name, products.Last().Name);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5)]
        public async void GetProductsByGenreFailsWithBadID(int id)
        {
            await Assert.ThrowsAsync<Exception>(() => _controller.ProductsByGenre(id, 1, 3));
        }
        [Fact]
        public async void GetProductsBySearchCriteriaWorks()
        {
            var products = await _productService.GetProductsBySearch(1, 3, 1, "Product1", 10, 60);
            var result = await _controller.SearchProducts("Product1", 20, 60, 1, 1, 3) as ViewResult;
            var model = result.Model as ProductPageViewModel;

            Assert.Equal(products, model.Products);
            Assert.Equal(products.First().Name, model.Products.First().Name);
            Assert.Equal(products.Last().Name, model.Products.Last().Name);
        }
        [Theory]
        [InlineData("product", -1, 60, 1)]
        [InlineData("product", 10, -1, 1)]
        [InlineData("product", -1, -1, 0)]
        public async void GetProductsBySearchCriteriaFails(string name, int? minprice, int? maxprice, int genreID)
        {
            await Assert.ThrowsAsync<Exception>(() => _controller.SearchProducts(name, minprice, maxprice, genreID, 1, 3));
        }

        private void SeedGenres()
        {
            var genres = new List<Genre>()
            {
                new Genre { Name = "Genre1" },
                new Genre { Name = "Genre2" },
                new Genre { Name = "Genre3" }
            };
            _context.Genres.AddRange(genres);
            _context.SaveChanges();
        }

        private void SeedProducts()
        {
            var products = new List<Product>()
            {
                new Product { Name = "Product1", Stock = 5, Price = 50, GenreID = 1 },
                new Product { Name = "Product2", Stock = 5, Price = 50, GenreID = 1 },
                new Product { Name = "Product3", Stock = 5, Price = 50, GenreID = 1 }
            };
            _context.Products.AddRange(products);
            _context.SaveChanges();
        }
        private async void SeedRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
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

        }
    }
}

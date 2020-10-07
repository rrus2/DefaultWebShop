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
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DefaultWebShopTests.GenreTests
{
    public class ProductsControllerTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private PasswordHasher<ApplicationUser> _passwordHasher;
        private ProductService _productService;
        private GenreService _genreService;
        private OrderService _orderService;
        private ProductsController _controller;
        private UserManager<ApplicationUser> _userManager;
        private UserStore<ApplicationUser> _userStore;
        private SignInManager<ApplicationUser> _signInManager;
        private HttpContextAccessor _contextAccessor;
        private IOptions<IdentityOptions> _identityOptions;
        private IdentityOptions _iOptions;
        private UserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

        public ProductsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _iOptions = new IdentityOptions();
            _iOptions.Password.RequireDigit = true;
            _identityOptions = _iOptions as IOptions<IdentityOptions>; 
            _passwordHasher = new PasswordHasher<ApplicationUser>();
            _contextAccessor = new HttpContextAccessor();
            _userStore =  new UserStore<ApplicationUser>(_context);
            _productService = new ProductService(_context, null);
            _genreService = new GenreService(_context);
            _orderService = new OrderService(_context);
            _userManager = new UserManager<ApplicationUser>(_userStore, null, _passwordHasher, null, null, null, null, null, null);
            _claimsFactory = new UserClaimsPrincipalFactory<ApplicationUser>(_userManager, _identityOptions);
            _signInManager = new SignInManager<ApplicationUser>(_userManager, _contextAccessor, _claimsFactory,  _identityOptions, null, null, null);

            _controller = new ProductsController(_productService, _genreService, _userManager, _orderService);

            SeedGenres();
            SeedProducts();
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
            var user = await _userManager.FindByNameAsync("pavel.ivanko@hotmail.com");

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
        private async void SeedUser()
        {
            var user = new ApplicationUser
            {
                Email = "pavel.ivanko@hotmail.com",
                UserName = "pavel.ivanko@hotmail.com",
                Birthdate = Convert.ToDateTime("30/07/1991")
            };

            var r = await _userManager.CreateAsync(user, "tesT1234567_!");

            if (!r.Succeeded)
                throw new Exception("LOL OK");

            var claim = new Claim("type", "value", "valueType", "pavel.ivanko@hotmail.com", "pavel.ivanko@hotmail.com");
            var claims = new List<Claim>() { claim };
            var ci = new ClaimsIdentity(claims);
            var p = new ClaimsPrincipal(ci);

            await _userManager.AddClaimAsync(user, claim);

            _controller.ControllerContext = new ControllerContext(new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            });

            UserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory = new UserClaimsPrincipalFactory<ApplicationUser>(_userManager, null);

            var ucpf = await userClaimsPrincipalFactory.CreateAsync(user);

            _controller.ControllerContext.HttpContext.User = ucpf;
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

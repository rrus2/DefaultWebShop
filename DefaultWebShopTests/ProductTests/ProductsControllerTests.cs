using DefaultWebShop.Controllers;
using DefaultWebShop.Migrations;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.GenreTests
{
    public class ProductsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private ProductService _productService;
        private GenreService _genreService;
        private OrderService _orderService;
        private ProductsController _controller;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _singInManager;
        private UserStore<ApplicationUser> _userStore;
        private HttpContextAccessor _contextAccessor;
        private UserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private IdentityOptions _identityOptions;

        public ProductsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _identityOptions = new IdentityOptions();
            _contextAccessor = new HttpContextAccessor();
            _userStore =  new UserStore<ApplicationUser>(_context);
            _productService = new ProductService(_context, null);
            _genreService = new GenreService(_context);
            _orderService = new OrderService(_context);
            _userManager = new UserManager<ApplicationUser>(_userStore, null, null, null, null, null, null, null, null);
            _claimsFactory = new UserClaimsPrincipalFactory<ApplicationUser>(_userManager, null);
            _singInManager = new SignInManager<ApplicationUser>(_userManager, _contextAccessor, _claimsFactory, null, null, null, null);

            _controller = new ProductsController(_productService, _genreService, _userManager, _orderService);
            SeedGenres(_context);
            SeedProducts(_context);
            SeedUser(_context);
            
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
            var user = await _userManager.FindByNameAsync("pavel.ivanko@hotmail.com");
            await _singInManager.SignInAsync(user, true);

        }
        private void SeedGenres(ApplicationDbContext context)
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

        private void SeedProducts(ApplicationDbContext context)
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
        private void SeedUser(ApplicationDbContext _context)
        {
            var user = new ApplicationUser
            {
                Email = "pavel.ivanko@hotmail.com",
                UserName = "pavel.ivanko@hotmail.com",
                Birthdate = Convert.ToDateTime("30/07/1991")
            };

            _userManager.CreateAsync(user, "test12345_!");
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

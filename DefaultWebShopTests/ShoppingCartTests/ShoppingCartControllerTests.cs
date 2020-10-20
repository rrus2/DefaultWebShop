using DefaultWebShop.Controllers;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShopTests.Fixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.ShoppingCartTests
{
    public class ShoppingCartControllerTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ServiceProvider _provider;
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private ShoppingCartController _controller;
        public ShoppingCartControllerTests()
        {
            _provider = new DbFixture().Provider;

            _context = _provider.GetService<ApplicationDbContext>();
            _userManager = _provider.GetService<UserManager<ApplicationUser>>();
            _productService = new ProductService(_context, null);
            _orderService = new OrderService(_context);
            _shoppingCartService = new ShoppingCartService(_context, _userManager);

            _controller = new ShoppingCartController(_shoppingCartService);

            SeedProducts();
            SeedUser();
            SeedShoppingCart();
        }

        [Fact]
        public async void ShoppingCartIndexWorks()
        {
            var result = await _controller.Index() as ViewResult;
            var model = result.Model as IEnumerable<ShoppingCart>;

            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");
            var cartItems = await _shoppingCartService.GetCartItems(user.UserName);

            Assert.Equal(model, cartItems);
            Assert.Equal(model.First(), cartItems.First());
            Assert.Equal(model.Last(), cartItems.Last());
        }

        [Fact]
        public async void ShoppingCartDeleteCartItemWorks()
        {
            var result = await _controller.DeleteFromCart(1) as ViewResult;
        }

        [Fact]
        public async void ShoppingCartAddToCartWorks()
        {
            var product = _context.Products.First();
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var result = await _controller.AddToCart(product.ProductID, 2) as ViewResult;
            var model = result.Model as ShoppingCart;

            Assert.NotNull(model);
            Assert.Equal(model.ApplicationUser, user);
            Assert.Equal(model.Product, product);
        }

        private void SeedProducts()
        {
            var products = new List<Product>()
            {
                new Product { Name = "Adidas", Price = 50, Stock = 10, GenreID = 1},
                new Product { Name = "Nike", Price = 50, Stock = 10, GenreID = 1},
                new Product { Name = "Puma", Price = 50, Stock = 10, GenreID = 1},
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        private async void SeedUser()
        {
            var user = new ApplicationUser
            {
                UserName = "pavel@hotmail.com",
                Email = "pavel@hotmail.com",
                Birthdate = Convert.ToDateTime("30/07/1991")
            };

            await _userManager.CreateAsync(user, "testT_12345!");

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

        private async void SeedShoppingCart()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var product1 = _context.Products.First();
            var product2 = _context.Products.Last();

            await _shoppingCartService.AddToCart(product1.ProductID, user.UserName, 1);
            await _shoppingCartService.AddToCart(product2.ProductID, user.UserName, 1);
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}

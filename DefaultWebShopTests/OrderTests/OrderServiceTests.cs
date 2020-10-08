using DefaultWebShop.Migrations;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShopTests.Fixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.OrderTests
{
    public class OrderServiceTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private OrderService _orderService;
        private UserManager<ApplicationUser> _userManager;
        public OrderServiceTests(DbFixture fixture)
        {
            _context = fixture.Provider.GetService<ApplicationDbContext>();
            _context.Database.EnsureCreated();

            _orderService = new OrderService(_context);
            _userManager = fixture.Provider.GetService<UserManager<ApplicationUser>>();

            if(_context.Genres.Count() == 0)
                SeedGenres();
            if(_context.Products.Count() == 0)
                SeedProducts();
            if (_context.Users.Count() == 0)
                SeedUser();
        }

        [Fact]
        public async void CreateOrderWorks()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");
            var product = _context.Products.First();

            var order = await _orderService.CreateOrder(user.Id, product.ProductID, 2);

            Assert.NotNull(order);
            Assert.Equal(user.Email, order.ApplicationUser.Email);
            Assert.Equal(product.Name, order.Product.Name);
        }
        [Theory]
        [InlineData("pavel@hotmail.com", 0, 1)]
        [InlineData("pavel@hotmail.com", -1, 1)]
        [InlineData("pavel@hotmail.com", 0, -1)]
        [InlineData("pavel@hotmail.com", 1, 0)]
        [InlineData("pavel@hotmail.com", 0, 0)]
        [InlineData("pavel@hotmail.com", -1, -1)]
        public async void CreateOrderFails(string name, int productid, int amount)
        {
            var user = await _userManager.FindByNameAsync(name);
            await Assert.ThrowsAsync<Exception>(() => _orderService.CreateOrder(user.Id, productid, amount));
        }
        [Fact]
        public async void DeleteOrderWorks()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");
            var product = _context.Products.First();

            var order = await _orderService.CreateOrder(user.Id, product.ProductID, 2);
            await _orderService.DeleteOrder(order.OrderID);

            var orders = await _orderService.GetOrders();

            Assert.Empty(orders);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        public async void DeleteOrderFails(int orderid)
        {
            await Assert.ThrowsAsync<Exception>(() => _orderService.DeleteOrder(orderid));
        }

        [Fact]
        public async void GetOrdersWorks()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");
            var product1 = _context.Products.First();
            var product2 = _context.Products.Last();

            await _orderService.CreateOrder(user.Id, product1.ProductID, 2);
            await _orderService.CreateOrder(user.Id, product2.ProductID, 1);

            var order = await _orderService.GetOrders();

            Assert.Equal(2, order.Count());
        }
        [Fact]
        public async void GetOrdersByUserID()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");
            var product1 = _context.Products.First();
            var product2 = _context.Products.Last();

            await _orderService.CreateOrder(user.Id, product1.ProductID, 2);
            await _orderService.CreateOrder(user.Id, product2.ProductID, 1);

            var ordersByUserID = await _orderService.GetOrdersByUser(user.Id);

            Assert.Equal(ordersByUserID.First().ApplicationUser.UserName, user.UserName);
            Assert.Equal(ordersByUserID.First().Product.Name, product1.Name);
            Assert.Equal(ordersByUserID.Last().Product.Name, product2.Name);
        }
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async void GetOrdersByUserIDFails(string id)
        {
            await Assert.ThrowsAsync<Exception>(() => _orderService.GetOrdersByUser(id));
        }
        private void SeedGenres()
        {
            var genres = new List<Genre>()
            {
                new Genre { Name = "Sports" },
                new Genre { Name = "Dunno"}
            };
            _context.Genres.AddRange(genres);
            _context.SaveChanges();
        }
        private void SeedProducts()
        {
            var products = new List<Product>() 
            {
                new Product { Name = "Nike", Price = 50, Stock = 5, GenreID = 1},
                new Product { Name = "Adidas", Price = 50, Stock = 5, GenreID = 1},
                new Product { Name = "Something", Price = 50, Stock = 5, GenreID = 1},
                new Product { Name = "SomethingElse", Price = 50, Stock = 5, GenreID = 2}
            };
            _context.Products.AddRange(products);
            _context.SaveChanges();
        }
        private async void SeedUser()
        {
            var user = new ApplicationUser { UserName = "pavel@hotmail.com", Birthdate = Convert.ToDateTime("30/07/1991") };
            var result = await _userManager.CreateAsync(user, "testT_12345!");
            if (!result.Succeeded)
                throw new Exception("LOL OK");
        }
        public void Dispose()
        {
        }
    }
}

﻿using DefaultWebShop.Migrations;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShopTests.Fixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.ShoppingCartTests
{
    public class ShoppingCartServiceTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ServiceProvider _provider;
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartServiceTests()
        {
            _provider = new DbFixture().Provider;

            _context = _provider.GetService<ApplicationDbContext>();
            _orderService = new OrderService(_context);
            _productService = new ProductService(_context, null);

            _userManager = _provider.GetService<UserManager<ApplicationUser>>();

            _shoppingCartService = new ShoppingCartService(_context, _userManager);

            SeedUser();
            SeedGenres();
            SeedProducts();
            SeedCart();
        }

        [Fact]
        public async void AddToCartWorks()
        {
            var product = await _productService.GetProduct(1);
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var order = await _orderService.CreateOrder(user.Id, product.ProductID, 1);

            Assert.Equal(order.ProductID, product.ProductID);
        }

        [Theory]
        [InlineData(100, 1)]
        [InlineData(0, 2)]
        [InlineData(-1, 3)]
        public async void AddToCartFails(int amount, int productid)
        {
            var product = await _productService.GetProduct(productid);
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            await Assert.ThrowsAsync<Exception>(() => _orderService.CreateOrder(user.Id, product.ProductID, amount));
        }

        [Fact]
        public async void DeleteFromCartWorks()
        {
            var product = await _productService.GetProduct(1);
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var cartItem = await _shoppingCartService.AddToCart(product.ProductID, user.UserName, 1);

            await _shoppingCartService.DeleteFromCart(cartItem.ShoppingCartID);

            await Assert.ThrowsAsync<Exception>(() => _shoppingCartService.DeleteFromCart(cartItem.ShoppingCartID));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public async void DeleteFromCartFails(int id)
        {
            await Assert.ThrowsAsync<Exception>(() => _shoppingCartService.DeleteFromCart(id));
        }

        [Fact]
        public async void GetCartItemsWorks()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var product1 = _context.Products.First();
            var product2 = _context.Products.Last();

            await _shoppingCartService.AddToCart(product1.ProductID, user.UserName, 1);
            await _shoppingCartService.AddToCart(product2.ProductID, user.UserName, 2);

            var cartItems = await _shoppingCartService.GetCartItems(user.UserName);

            Assert.NotNull(cartItems);
            Assert.Equal(product1.Name, cartItems.First().Product.Name);
            Assert.Equal(product2.Name, cartItems.Last().Product.Name);
        }

        [Fact]
        public async void GetCartItemsFails()
        {
            await Assert.ThrowsAsync<Exception>(() => _shoppingCartService.GetCartItems("lol"));
        }

        [Fact]
        public async void BuyAllWorks()
        {
            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");
            await _shoppingCartService.BuyAll(user.Id);

            Assert.Empty(_context.ShoppingCarts);
            Assert.NotNull(_context.Orders);
            Assert.Equal(_context.Orders.First().ApplicationUser, user);
        }
        private void SeedGenres()
        {
            var genres = new List<Genre>()
            {
                new Genre { Name = "Genre1"},
                new Genre { Name = "Genre2"}
            };

            _context.Genres.AddRange(genres);
            _context.SaveChanges();
        }

        private void SeedProducts()
        {
            var products = new List<Product>()
            {
                new Product { Name = "Adidas", Price = 50, Stock = 5, GenreID = 1},
                new Product { Name = "Nike", Price = 50, Stock = 10, GenreID = 2},
                new Product { Name = "Puma", Price = 60, Stock = 15, GenreID = 1}
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        private async void SeedUser()
        {
            var user = new ApplicationUser
            {
                Email = "pavel@hotmail.com",
                UserName = "pavel@hotmail.com",
                Birthdate = Convert.ToDateTime("30/07/1991")
            };

            await _userManager.CreateAsync(user, "testT_12345!");
        }
        private async void SeedCart()
        {
            var product1 = _context.Products.FirstOrDefault(x => x.Name == "Adidas");
            var product2 = _context.Products.FirstOrDefault(x => x.Name == "Nike");
            var product3 = _context.Products.FirstOrDefault(x => x.Name == "Puma");

            var user = await _userManager.FindByNameAsync("pavel@hotmail.com");

            var carts = new List<ShoppingCart>()
            {
                new ShoppingCart { ApplicationUser = user, Product = product1, Amount = 2 },
                new ShoppingCart { ApplicationUser = user, Product = product2, Amount = 3 },
                new ShoppingCart { ApplicationUser = user, Product = product3, Amount = 5 }
            };

            _context.ShoppingCarts.AddRange(carts);
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}

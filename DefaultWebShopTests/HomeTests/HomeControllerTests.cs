using DefaultWebShop.Controllers;
using DefaultWebShop.Migrations;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using DefaultWebShopTests.Fixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.HomeTests
{
    public class HomeControllerTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private ProductService _productService;
        private ServiceProvider _provider;
        private HomeController _controller;
        public HomeControllerTests()
        {
            _provider = new DbFixture().Provider;

            _context = _provider.GetService<ApplicationDbContext>();

            _productService = new ProductService(_context, null);

            _controller = new HomeController(null, _productService);

            SeedGenres();
            SeedProducts();
        }

        [Fact]
        public async void IndexPageWorks()
        {
            var products = _context.Products.ToList();
            var result = await _controller.Index() as ViewResult;
            var model = result.Model as List<Product>;

            Assert.Contains(model.First(), products);
            Assert.Contains(model.Last(), products);
        }

        private void SeedGenres()
        {
            var genres = new List<Genre>()
            {
                new Genre { Name = "Genre1"},
                new Genre { Name = "Genre2"},
                new Genre { Name = "Genre3"}
            };

            _context.Genres.AddRange(genres);
            _context.SaveChanges();
        }

        private void SeedProducts()
        {
            var products = new List<Product>()
            {
                new Product { Name = "Nike", Price = 50, Stock = 5, GenreID = 1 },
                new Product { Name = "Adidas", Price = 50, Stock = 5, GenreID = 1 },
                new Product { Name = "Puma", Price = 50, Stock = 5, GenreID = 1 },
                new Product { Name = "OK", Price = 50, Stock = 5, GenreID = 2 },
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}

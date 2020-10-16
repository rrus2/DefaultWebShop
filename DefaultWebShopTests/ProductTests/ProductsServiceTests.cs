using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using DefaultWebShopTests.Fixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace DefaultWebShopTests.ProductTests
{
    public class ProductsServiceTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private ProductService _productService;
        private ServiceProvider _provider;
        public ProductsServiceTests()
        {
            _provider = new DbFixture().Provider;

            _context = _provider.GetService<ApplicationDbContext>();

            _context.Database.EnsureCreated();

            var genres = new List<Genre>()
            {
                new Genre { Name = "Genre1"},
                new Genre { Name = "Genre2"},
                new Genre { Name = "Genre3"}
            };

            _context.Genres.AddRange(genres);

            _productService = new ProductService(_context, null);
        }

        [Fact]
        public async void CreateProductWorks()
        {
            var productVM = new ProductViewModel
            {
                Name = "Product",
                Price = 50,
                Amount = 5,
                Stock = 5,
                GenreID = 1
            };

            var product = await _productService.CreateProduct(productVM, null);

            Assert.NotNull(product);

        }

        [Theory]
        [InlineData("", 50, 5, 5, 1)]
        [InlineData("Product", 0, 5,5,1)]
        [InlineData("Product", 50, 0, 5, 1)]
        [InlineData("Product", 50, 5, 0, 1)]
        [InlineData("Product", 50, 5, 5, 0)]
        [InlineData(null, 50, 5, 5, 1)]
        [InlineData(null, -1, 5, 5, 1)]
        [InlineData(null, -1, 0, -1, int.MinValue)]

        public async void CreateProductsShouldFail(string name, double price, int amount, int stock, int GenreID)
        {
            var productVM = new ProductViewModel
            {
                Name = name,
                Price = price,
                Amount = amount,
                Stock = stock,
                GenreID = GenreID
            };

            await Assert.ThrowsAsync<Exception>(() => _productService.CreateProduct(productVM, null));
        }

        [Fact]
        public async void DeleteProductShouldWork()
        {
            var productVM = new ProductViewModel
            {
                Name = "Product",
                Price = 50,
                Amount = 5,
                Stock = 5,
                GenreID = 1
            };

            var product = await _productService.CreateProduct(productVM, null);
            await _productService.DeleteProduct(product.ProductID);

            var products = await _productService.GetProducts(null, 5);

            Assert.DoesNotContain(product, products);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public async void DeleteProductShouldNotWork(int id)
        {
            await Assert.ThrowsAsync<Exception>(() => _productService.DeleteProduct(id));
        }

        [Fact]
        public async void GetCountShouldWork()
        {
            var productVM1 = new Product
            {
                Name = "Product1",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };
            var productVM2 = new Product
            {
                Name = "Product2",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };
            var productVM3 = new Product
            {
                Name = "Product3",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };

            _context.Products.Add(productVM1);
            _context.Products.Add(productVM2);
            _context.Products.Add(productVM3);
            _context.SaveChanges();

            var count = await _productService.GetCount();

            Assert.Equal(3, count);
        }

        [Fact]
        public async void GetProductShouldWork()
        {
            var productVM1 = new ProductViewModel
            {
                Name = "Product1",
                Price = 50,
                Stock = 5,
                GenreID = 1,
                Amount = 5
            };

            var product = await _productService.CreateProduct(productVM1, null);

            var products = await _productService.GetProducts(1, 5);
            var productToGet = await _productService.GetProduct(product.ProductID);

            Assert.NotNull(productToGet);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async void GetProductsShouldNotWorkWithBadID(int id)
        {
            await Assert.ThrowsAsync<Exception>(() => _productService.GetProduct(id));
        }

        [Fact]
        public async void GetProductShouldNotWorkWithNonExistingProductID()
        {
            await Assert.ThrowsAsync<Exception>(() => _productService.GetProduct(1));
        }

        [Fact]
        public async void GetProductsShouldWork()
        {
            var productVM1 = new Product
            {
                Name = "Product1",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };
            var productVM2 = new Product
            {
                Name = "Product2",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };
            var productVM3 = new Product
            {
                Name = "Product3",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };

            _context.Products.Add(productVM1);
            _context.Products.Add(productVM2);
            _context.Products.Add(productVM3);
            _context.SaveChanges();

            var products = await _productService.GetProducts(null, 5);

            Assert.Equal(products.First().Name, productVM1.Name);
        }
        [Theory]
        [InlineData("prod", null, null, 0)]
        public async void GetProductsShouldWorkWithSearchParameters(string name, int? minprice, int? maxprice, int genreid)
        {
            var products = await _productService.GetCountBySearch(genreid, name, minprice, maxprice);
        }
        [Fact]
        public async void UpdateProductShouldWork()
        {
            var product = new Product
            {
                Name = "Nike",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };

            var productToUpdate = _context.Products.Add(product);
            _context.SaveChanges();

            var productVM = new ProductViewModel
            {
                Name = "Adidas",
                Price = 25,
                Stock = 3,
                GenreID = 2,
                Amount = 1
            };

            var updatedProduct = await _productService.UpdateProduct(productToUpdate.Entity.ProductID, productVM);

            Assert.Equal(productToUpdate.Entity, updatedProduct);
            Assert.Equal(productToUpdate.Entity.Name, updatedProduct.Name);
            Assert.Equal(productToUpdate.Entity.Price, updatedProduct.Price);
        }

        [Theory]
        [InlineData("", 50, 5, 5, 1)]
        [InlineData("Product", 0, 5, 5, 1)]
        [InlineData("Product", 50, 0, 5, 1)]
        [InlineData("Product", 50, 5, 0, 1)]
        [InlineData("Product", 50, 5, 5, 0)]
        [InlineData(null, 50, 5, 5, 1)]
        [InlineData(null, -1, 5, 5, 1)]
        [InlineData(null, -1, 0, -1, int.MinValue)]
        public async void UpdateProductShouldFailWithBadData(string name, double price, int amount, int stock, int GenreID)
        {
            var product = new Product
            {
                Name = "Nike",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };

            var productToUpdate = _context.Products.Add(product);
            _context.SaveChanges();

            var productVM = new ProductViewModel
            {
                Name = name,
                Price = price,
                Stock = stock,
                GenreID = GenreID,
                Amount = amount
            };

            await Assert.ThrowsAsync<Exception>(() => _productService.UpdateProduct(productToUpdate.Entity.ProductID, productVM));
        }

        [Fact]
        public async void UpdateProductShouldFailWithNullModel()
        {
            var product = new Product
            {
                Name = "Nike",
                Price = 50,
                Stock = 5,
                GenreID = 1
            };

            var productToUpdate = _context.Products.Add(product);
            _context.SaveChanges();

            var productVM = new ProductViewModel();

            await Assert.ThrowsAsync<Exception>(() => _productService.UpdateProduct(productToUpdate.Entity.ProductID, productVM));
        }
        public void Dispose()
        {
            _provider.Dispose(); 
        }
    }
}

﻿using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<Product> CreateProduct(ProductViewModel model, IFormFile file)
        {
            if (model == null)
                throw new Exception("ProductViewModel cannot be null");

            if (model.Name == null || model.Name == string.Empty)
                throw new Exception("Product name can not be null or empty");

            if (model.Price <= 0)
                throw new Exception("Product price can not be zero or less than");

            if (model.Stock <= 0)
                throw new Exception("Product stock can not be zero or less than");

            if (model.Amount <= 0)
                throw new Exception("Product amout can not be zero or less than");

            if (model.GenreID <= 0)
                throw new Exception("Product genre must be provided");

            var product = new Product { Name = model.Name, Price = model.Price, ImagePath = model.ImagePath, Stock = model.Stock, GenreID = model.GenreID };
            if(file != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images");
                var fileName = Guid.NewGuid().ToString() + file.FileName;
                using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    var imagePath = "images/" + fileName;
                    product.ImagePath = imagePath;
                }
            }
            try
            {
                await _context.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return product;
        }

        public async Task<Product> DeleteProduct(int id)
        {
            if (id == 0 || id < 0)
                throw new Exception("Deleting productID 0 not allowed");
            var product = await _context.Products.FindAsync(id);
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        public async Task<int> GetCount()
        {
            var products = await _context.Products.ToListAsync();
            return products.Count;
        }

        public async Task<int> GetCountByGenreID(int genreID)
        {
            var products = await _context.Products.Where(x => x.GenreID == genreID).ToListAsync();
            return products.Count;
        }

        public async Task<int> GetCountBySearch(int genreID, string name, int? minvalue, int? maxvalue)
        {
            var products = _context.Products.AsQueryable();
            if (genreID > 0)
                products = products.Where(x => x.GenreID == genreID);
            if (name != null && name != string.Empty)
                products = products.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            if (minvalue > 0)
                products = products.Where(x => x.Price >= minvalue);
            if (maxvalue > 0)
                products = products.Where(x => x.Price <= maxvalue);
            if (minvalue > 0 && maxvalue > 0)
                products = products.Where(x => x.Price > minvalue && x.Price < maxvalue);
            return await products.CountAsync();
        }

        public async Task<Product> GetProduct(int id)
        {
            if (id == 0 || id < 0)
                throw new Exception("Product id cannot be 0 or less than");
            var product = await _context.Products.Include(x => x.Genre).FirstOrDefaultAsync(x => x.ProductID == id);
            if (product == null)
                throw new Exception($"Product with id: {id} is null");
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            var products = _context.Products.Include(x => x.Genre).AsQueryable();
            if (name != null && name != string.Empty)
                products = products.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            return await products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts(int? pageNumber, int size)
        {
            var products = _context.Products.Include(x => x.Genre).AsQueryable();
            if (pageNumber != null)
                products = products.OrderBy(x => x.Name).Skip(((int)pageNumber - 1) * size).Take(size);
            var productsToReturn = await products.ToListAsync();
            
            return productsToReturn;
        }

        public async Task<IEnumerable<Product>> GetProductsByGenre(int? pageNumber, int size, int genreID)
        {
            var count = await GetCount();
            if (genreID == 0 || genreID < 0 || genreID > count)
                throw new Exception("Genre can not be 0 or less than");
            var products = _context.Products.Include(x => x.Genre).Where(x => x.GenreID == genreID).AsQueryable();
            if (pageNumber != null)
                products = products.OrderBy(x => x.Name).Skip(((int)pageNumber - 1) * (int)size).Take((int)size);

            return await products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsBySearch(int? pageNumber, int size, int genreID, string name, int? minvalue, int? maxvalue)
        {
            if (minvalue < 0)
                throw new Exception("Minimum value can not be less than 0");
            if (maxvalue < 0)
                throw new Exception("Maximum value can not be less than 0");
            var products = _context.Products.Include(x => x.Genre).AsQueryable();
            if (name != null && name != string.Empty)
                products = products.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            if (minvalue > 0 && minvalue != null)
                products = products.Where(x => x.Price >= minvalue);
            if (maxvalue > 0 && maxvalue != null)
                products = products.Where(x => x.Price <= maxvalue);
            if (minvalue > 0 && maxvalue > 0 && minvalue != null && maxvalue != null)
                products = products.Where(x => x.Price >= minvalue && x.Price <= maxvalue);
            if (genreID > 0)
                products = products.Where(x => x.GenreID == genreID);
            if (pageNumber != null)
                products = products.OrderBy(x => x.Name).Skip(((int)pageNumber - 1) * (int)size).Take((int)size);

            return await products.ToListAsync();
        }

        public async Task<Product> UpdateProduct(int id, ProductViewModel model)
        {
            if (id == 0 || id < 0)
                throw new Exception("Can not update product with id 0");

            if (model.Name == null || model.Name == string.Empty)
                throw new Exception("Product name can not be null or empty");

            if (model.Price <= 0)
                throw new Exception("Product price can not be zero or less than");

            if (model.Stock <= 0)
                throw new Exception("Product stock can not be zero or less than");

            if (model.Amount <= 0)
                throw new Exception("Product amout can not be zero or less than");

            if (model.GenreID <= 0)
                throw new Exception("Product genre must be provided");

            var product = await _context.Products.Include(x => x.Genre).FirstOrDefaultAsync(x => x.ProductID == id);

            if (product == null)
                throw new Exception($"Product with id {id} is null");

            try
            {
                product.Name = model.Name;
                product.Price = model.Price;
                product.Stock = model.Stock;
                product.ImagePath = model.ImagePath;
                product.GenreID = model.GenreID;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }
    }
}

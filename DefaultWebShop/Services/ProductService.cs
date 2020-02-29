using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product> CreateProduct(ProductViewModel model)
        {
            if (model == null)
                throw new Exception("ProductViewModel cannot be null");
            var product = new Product { Name = model.Name, Price = model.Price, ImagePath = model.ImagePath, Stock = model.Stock, GenreID = model.GenreID };
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

        public async Task<Product> GetProduct(int id)
        {
            if (id == 0 || id < 0)
                throw new Exception("Product id cannot be 0 or less than");
            var product = await _context.Products.Include(x => x.Genre).FirstOrDefaultAsync(x => x.ProductID == id);
            if (product == null)
                throw new Exception($"Product with id: {id} is null");
            return product;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products = await _context.Products.Include(x => x.Genre).ToListAsync();
            return products;
        }

        public async Task<Product> UpdateProduct(int id, ProductViewModel model)
        {
            if (id == 0 || id < 0)
                throw new Exception("Can not update product with id 0");
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
    }
}

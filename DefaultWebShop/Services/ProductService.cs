using DefaultWebShop.Models;
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

        public async Task<Product> GetProduct(int id)
        {
            if (id == 0 || id < 0)
                throw new Exception("Product id cannot be 0 or less than");
            var product = await _context.Products.Include(x => x.Genre).FirstOrDefaultAsync(x => x.ProductID == id);
            if (product == null)
                throw new Exception($"Product with id: {id} is null");
            return product;
        }

        public async Task<IEnumerable<Product>> GetProducts(int? pageNumber, int size)
        {
            var products = _context.Products.Include(x => x.Genre).AsEnumerable();
            if (pageNumber != null)
            {
                products = products.OrderBy(x => x.Name).Skip(((int)pageNumber - 1) * (int)size).Take((int)size);
            }
            return products.ToList();
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

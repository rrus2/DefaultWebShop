using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
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
        public Task<Product> CreateProduct(ProductViewModel product)
        {
            throw new NotImplementedException();
        }

        public Task<Product> DeleteProduct(ProductViewModel product)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetProducts()
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateProduct(int id, ProductViewModel product)
        {
            throw new NotImplementedException();
        }
    }
}

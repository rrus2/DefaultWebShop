using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProducts(int? pageNumber, int size);
        Task<int> GetCount();
        Task<Product> GetProduct(int id);
        Task<Product> CreateProduct(ProductViewModel model, IFormFile file);
        Task<Product> UpdateProduct(int id, ProductViewModel model);
        Task<Product> DeleteProduct(int id);
    }
}

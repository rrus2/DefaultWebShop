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
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProducts(int? pageNumber, int size);
        Task<IEnumerable<Product>> GetProductsByGenre(int? pageNumber, int size, int genreID);
        Task<IEnumerable<Product>> GetProductsBySearch(int? pageNumber, int size, int genreID, string name, int? minvalue, int? maxvalue);
        Task<int> GetCount();
        Task<int> GetCountByGenreID(int genreID);
        Task<int> GetCountBySearch(int genreID, string name, int? minvalue, int? maxvalue);
        Task<Product> GetProduct(int id);
        Task<IEnumerable<Product>> GetProductsByName(string name);
        Task<Product> CreateProduct(ProductViewModel model, IFormFile file);
        Task<Product> UpdateProduct(int id, ProductViewModel model);
        Task<Product> DeleteProduct(int id);
    }
}

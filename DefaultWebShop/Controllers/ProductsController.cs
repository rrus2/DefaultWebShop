using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DefaultWebShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IGenreService _genreService;
        public ProductsController(IProductService productService, IGenreService genreService)
        {
            _productService = productService;
            _genreService = genreService;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProducts();

            return View(products);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProduct(id);
            var model = new ProductViewModel { ProductID=product.ProductID, Name = product.Name, Price = product.Price, Stock = product.Stock, ImagePath = product.ImagePath };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadGenres();
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model, IFormCollection collection)
        {
            var file = collection.Files[0];
            if (!ModelState.IsValid)
            {
                await LoadGenres();
                return View(model);
            }

            await _productService.CreateProduct(model, file);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ProductsByGenre(int id)
        {
            var genre = await _genreService.GetGenre(id);
            var products = await _productService.GetProducts();
            var genreproducts = products.Where(x => x.GenreID == genre.GenreID);
            return View(genreproducts);
        }
        private async Task LoadGenres()
        {
            var genres = await _genreService.GetGenres();
            ViewBag.Genres = new SelectList(genres, "GenreID", "Name");
        }
    }
}
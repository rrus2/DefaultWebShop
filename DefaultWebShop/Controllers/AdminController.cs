using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DefaultWebShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IProductService _productService;
        public AdminController(IGenreService genreService, IProductService productService)
        {
            _genreService = genreService;
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CreateProduct()
        {
            await LoadGenres();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                await LoadGenres();
                return View(model);
            }
            await _productService.CreateProduct(model, image);
            return View(nameof(Index));
        }
        private async Task LoadGenres()
        {
            var genres = await _genreService.GetGenres();
            ViewBag.Genres = new SelectList(genres, "GenreID", "Name");
        }
    }
}
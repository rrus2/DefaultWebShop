using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DefaultWebShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IGenreService _genreService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        public ProductsController(IProductService productService, 
            IGenreService genreService, 
            UserManager<ApplicationUser> userManager,
            IOrderService orderService)
        {
            _productService = productService;
            _genreService = genreService;
            _userManager = userManager;
            _orderService = orderService;
        }
        public async Task<IActionResult> Index(int? pageNumber = 1, int size = 3)
        {
            var products = await _productService.GetProducts(pageNumber, size);
            var model = new ProductPageViewModel 
            { 
                Count = await _productService.GetCount(), 
                Products = products, 
                CurrentPage = (int)pageNumber 
            };
            return View(model);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProduct(id);
            var model = new ProductViewModel { ProductID=product.ProductID, Name = product.Name, Price = product.Price, Stock = product.Stock, ImagePath = product.ImagePath };
            CalculateAmount(product.Stock);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int productid, int amount)
        {
            var user = await GetUser(HttpContext.User.Identity.AuthenticationType);
            await _orderService.CreateOrder(user.Id, productid, amount);
            return View();
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
            IFormFile file;
            if (collection != null)
                file = collection.Files[0];
            else
                file = null;

            if (!ModelState.IsValid)
            {
                await LoadGenres();
                return View(model);
            }

            await _productService.CreateProduct(model, file);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ProductsByGenre(int id, int? pageNumber = 1, int size = 3)
        {
            var products = await _productService.GetProductsByGenre(pageNumber, size, id);
            var genreproductsviewmodel = new ProductPageViewModel 
            { 
                Count = await _productService.GetCountByGenreID(id), 
                CurrentPage = (int)pageNumber, 
                Products = products 
            };
            return View(genreproductsviewmodel);
        }
        public async Task<IActionResult> SearchProducts(string name, int? minvalue, int? maxvalue, int genreID, int? pageNumber = 1, int size = 3)
        {
            var products = await _productService.GetProductsBySearch(pageNumber, size, genreID, name, minvalue, maxvalue);
            var searchproductsviewmodel = new ProductPageViewModel
            {
                Count = await _productService.GetCountBySearch(genreID, name, (int)minvalue, (int)maxvalue),
                CurrentPage = (int)pageNumber,
                Products = products,
                Name = name,
                GenreID = genreID,
                MinValue = minvalue,
                MaxValue = maxvalue

            };
            return View(searchproductsviewmodel);

        }
        private async Task LoadGenres()
        {
            var genres = await _genreService.GetGenres();
            ViewBag.Genres = new SelectList(genres, "GenreID", "Name");
        }
        private void CalculateAmount(int stock)
        {
            var list = new List<int>();
            for (int i = 1; i < stock+1; i++)
            {
                list.Add(i);
            }
            var select = new SelectList(list);
            ViewBag.Amount = select;
        }
        private async Task<ApplicationUser> GetUser(string claim)
        {
            var user = await _userManager.FindByNameAsync(claim);
            return user;
        }
    }
}
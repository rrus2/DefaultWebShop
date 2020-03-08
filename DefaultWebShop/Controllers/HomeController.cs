using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DefaultWebShop.Models;
using Microsoft.AspNetCore.Identity;
using DefaultWebShop.Services;

namespace DefaultWebShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var count = await _productService.GetCount();
            var products = await _productService.GetProducts(0, count);
            var productsRandom = RandomizeProducts(products);
            return View(productsRandom);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Product> RandomizeProducts(IEnumerable<Product> products)
        {
            // generate 5 random numbers
            var random = new Random();
            var listofnums = new HashSet<int>();
            for (int i = 1; i < 6; i++)
            {
                var num = random.Next(0, products.Count());
                listofnums.Add(num);
            }

            //pick random products with int hashset
            var list = new List<Product>();
            foreach (var number in listofnums)
            {
                list.Add(products.ToList()[number]);
            }

            return list;
        }
    }
}

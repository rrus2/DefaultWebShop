using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultWebShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DefaultWebShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claim = HttpContext.User;
            var items = await _shoppingCartService.GetCartItems(claim);
            return View(items);
        }

        public async Task<IActionResult> DeleteFromCart(int id)
        {
            await _shoppingCartService.DeleteFromCart(id);
            return View(nameof(Index));
        }
    }
}
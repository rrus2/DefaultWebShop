using DefaultWebShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartService(IOrderService orderService, IProductService productService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _productService = productService;
            _context = context;
            _userManager = userManager;
        }
        public async Task<ShoppingCart> AddToCart(int productid, ClaimsPrincipal claim, int amount)
        {
            var product = await _productService.GetProduct(productid);
            if (product == null)
                throw new Exception("Error adding product tot cart");
            var user = await _userManager.GetUserAsync(claim);
            if (product == null)
                throw new Exception("Error getting user for cart");
            var shoppingcart = new ShoppingCart { ApplicationUser = user, Product = product, Amount = amount, TotalPrice = amount * product.Price };
            try
            {
                _context.ShoppingCarts.Add(shoppingcart);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return shoppingcart;
        }

        public async Task<ShoppingCart> DeleteFromCart(int id)
        {
            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.ShoppingCartID == id);
            if (cart == null)
                throw new Exception("Cart does not exist");
            try
            {
                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return cart;
        }

        public async Task<IEnumerable<ShoppingCart>> GetCartItems(ClaimsPrincipal claim)
        {
            var user = await _userManager.GetUserAsync(claim);
            var items = await _context.ShoppingCarts.Include(x => x.Product).Include(x => x.ApplicationUser).Where(x => x.ApplicationUser == user).ToListAsync();
            return items;
        }
    }
}

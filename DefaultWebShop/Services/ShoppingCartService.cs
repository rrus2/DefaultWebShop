using DefaultWebShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account.Manage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ShoppingCart> AddToCart(int productid, string name, int amount)
        {
            var product = _context.Products.FirstOrDefault(x => x.ProductID == productid);
            if (amount <= 0)
                throw new Exception("Amount can not be 0 or less than");
            if (amount > product.Stock)
                throw new Exception("Amount can not be greater than the stock of the product");
            if (product == null)
                throw new Exception("Error adding product tot cart");
            var user = await _userManager.FindByNameAsync(name);
            if (product == null)
                throw new Exception("Error getting user for cart");
            var shoppingcart = new ShoppingCart { ApplicationUser = user, Product = product, Amount = amount, TotalPrice = amount * product.Price };
            try
            {
                if(!_context.ShoppingCarts.Contains(shoppingcart))
                    _context.ShoppingCarts.Add(shoppingcart);
                else
                {
                    var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Product.ProductID == product.ProductID);
                    cart.Amount += amount;
                    cart.TotalPrice = cart.Amount * cart.Product.Price;
                }
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

        public async Task<IEnumerable<ShoppingCart>> GetCartItems(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
                throw new Exception("The user has to be an existing one");
            var items = await _context.ShoppingCarts.Include(x => x.Product).Include(x => x.ApplicationUser).Where(x => x.ApplicationUser == user).ToListAsync();
            return items;
        }
    }
}

using DefaultWebShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCart> AddToCart(int productid, string name, int amount);
        Task<IEnumerable<ShoppingCart>> GetCartItems(string name);
        Task<ShoppingCart> DeleteFromCart(int id);
        Task BuyAll(string userid);
    }
}

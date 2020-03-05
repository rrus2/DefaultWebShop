using DefaultWebShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public class OrderService : IOrderService
    {
        private ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Order> CreateOrder(string userid, int productid, int amount)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userid);
            if (user == null)
                throw new Exception("User does not exist for this order");
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductID == productid);
            if (product == null)
                throw new Exception("Product does not exist for this order");
            var order = new Order { ApplicationUserID = userid, ApplicationUser = user, ProductID = productid, Product = product, Amount = amount };
            product.Stock -= amount;
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error with order: " + ex.Message);
            }
            
            return order;
        }

        public async Task<Order> DeleteOrder(int orderid)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderID == orderid);
            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }
    }
}

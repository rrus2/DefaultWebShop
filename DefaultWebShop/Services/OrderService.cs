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
            if (amount == 0 || amount < 0)
                throw new Exception("Amount can not be 0 or less than");
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
            if (orderid == 0 || orderid < 0)
                throw new Exception("OrderID can not be 0 or less than");
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

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUser(string id)
        {
            if (id == null || id == string.Empty)
                throw new Exception("UserIDD can not be null or empty");
            var ordersByUser = await GetOrders();

            return ordersByUser.Where(x => x.ApplicationUser.Id == id).ToList();
        }
    }
}

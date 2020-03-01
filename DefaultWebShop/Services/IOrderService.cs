using DefaultWebShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(string userid, int productid, int amount);
        Task<Order> DeleteOrder(int orderid);
    }
}

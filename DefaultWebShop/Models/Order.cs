using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string ApplicationUserID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
    }
}

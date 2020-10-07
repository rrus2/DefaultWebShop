using DefaultWebShop.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.ViewModels
{
    public class ProductPageViewModel : PageModel
    {
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 3;
        public bool HasPreviousPage
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }
        public int TotalPages => (int)Math.Ceiling(Count / (double)PageSize);
        public IEnumerable<Product> Products { get; set; }
        public int GenreID { get; set; }
        public string Name { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
    }
}

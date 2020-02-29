using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.ViewModels
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Product price is required")]
        [DataType(DataType.Currency)]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Product stock is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Stock cannot have negative value")]
        public int Stock { get; set; }
        [Required(ErrorMessage = "Image is required")]
        [DataType(DataType.Upload)]
        public string ImagePath { get; set; }
        [Required(ErrorMessage = "You must provide a genre")]
        [Range(1, int.MaxValue)]
        public int GenreID { get; set; }
    }
}

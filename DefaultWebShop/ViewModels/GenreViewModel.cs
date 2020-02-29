using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.ViewModels
{
    public class GenreViewModel
    {
        [Required(ErrorMessage = "Genre name is required")]
        public string Name { get; set; }
    }
}

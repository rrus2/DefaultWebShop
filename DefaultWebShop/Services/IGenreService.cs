using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetGenres();
        Task<Genre> GetGenre(int id);
        Task<Genre> CreateGenre(GenreViewModel model);
        Task<Genre> UpdateGenre(int id, GenreViewModel model);
        Task<Genre> DeleteGenre(int id);
    }
}

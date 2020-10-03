using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext _context;
        public GenreService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Genre> CreateGenre(GenreViewModel model)
        {
            if (model == null)
                throw new Exception("GenreViewModel cannot be null");
            if (model.Name == null || model.Name == "")
                throw new Exception("Name can not be null or empty");
            var genre = new Genre { Name = model.Name };
            try
            {
                await _context.Genres.AddAsync(genre);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return genre;
        }

        public async Task<Genre> DeleteGenre(int id)
        {
            if (id == 0 || id < 0)
                throw new Exception($"Cannot delete genre with id: {id}");
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.GenreID == id);
            try
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return genre;
        }

        public async Task<Genre> GetGenre(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.GenreID == id);
            if (genre == null)
                throw new Exception("Genre not found");
            return genre;
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return genres;
        }

        public async Task<Genre> UpdateGenre(int id, GenreViewModel model)
        {
            if (id == 0 || id < 0)
                throw new Exception($"Can not update genre with id {id}");

            if (model == null)
                throw new Exception("GenreViewModel is null");

            if (model.Name == null || model.Name == "")
                throw new Exception("Genre name can not be null or empty");

            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.GenreID == id);

            if (genre == null)
                throw new Exception("Genre not found");

            genre.Name = model.Name;
            try
            {
                _context.Genres.Update(genre);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return genre;
        }
    }
}

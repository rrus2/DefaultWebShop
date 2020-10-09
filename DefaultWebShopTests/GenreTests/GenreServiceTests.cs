using DefaultWebShop.Models;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using DefaultWebShopTests.Fixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DefaultWebShopTests.GenreTests
{
    public class GenreServiceTests : IDisposable, IClassFixture<DbFixture>
    {
        private readonly ApplicationDbContext _context;
        private GenreService _genreService;
        private ServiceProvider _provider;

        public GenreServiceTests()
        {
            _provider = new DbFixture().Provider;

            _context = _provider.GetService<ApplicationDbContext>();
            _context.Database.EnsureCreated();

            _genreService = new GenreService(_context);
        }
        [Fact]
        public async void AddGenreWorks()
        {
            // arrange
            var genreVM = new GenreViewModel { Name = "Genre" };

            // act
            await _genreService.CreateGenre(genreVM);
            var getGenres = await _genreService.GetGenres();

            // assert
            Assert.NotNull(getGenres);
            Assert.Equal("Genre", getGenres.First().Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async void AddGenreFailsWithFailName(string name)
        {
            var genreVM = new GenreViewModel { Name = name };

            await Assert.ThrowsAsync<Exception>(() => _genreService.CreateGenre(genreVM));
        }

        [Theory]
        [InlineData(null)]
        public async void AddGenreFailsWithNullModel(GenreViewModel genre)
        {
            await Assert.ThrowsAsync<Exception>(() => _genreService.CreateGenre(genre));
        }

        [Fact]
        public async void DeleteGenreWorks()
        {
            var genreVM1 = new GenreViewModel { Name = "Genre1" };
            var genreVM2 = new GenreViewModel { Name = "Genre2" };

            var genre = await _genreService.CreateGenre(genreVM1);
            await _genreService.CreateGenre(genreVM2);

            await _genreService.DeleteGenre(genre.GenreID);
            var genres = await _genreService.GetGenres();

            Assert.DoesNotContain(genre, genres);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async void DeleteGenreFailsWithFailID(int id)
        {
            await Assert.ThrowsAsync<Exception>(() => _genreService.DeleteGenre(id));
        }

        [Fact]
        public async void DeleteGenreFailsWithNonExistingGenre()
        {
            var model = new Genre { GenreID = 1, Name = "Genre" };
            await Assert.ThrowsAsync<Exception>(() => _genreService.DeleteGenre(model.GenreID));
        }

        [Fact]
        public async void GetGenreWorks()
        {
            var genreVM = new GenreViewModel { Name = "Genre" };
            var genre = await _genreService.CreateGenre(genreVM);

            var genreToGet = await _genreService.GetGenre(genre.GenreID);

            Assert.NotNull(genreToGet);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async void GetGenreFailsWithBadID(int id)
        {
            await Assert.ThrowsAsync<Exception>(() => _genreService.GetGenre(id));
        }

        [Fact]
        public async void GetGenreFailsWithNonExistingGenre()
        {
            var genre = new Genre { GenreID = 1, Name = "Genre" };
            await Assert.ThrowsAsync<Exception>(() => _genreService.GetGenre(genre.GenreID));
        }

        [Fact]
        public async void GetGenresWorks()
        {
            var genresToAdd = new List<Genre>
            {
                new Genre { GenreID = 1, Name = "Genre1"},
                new Genre { GenreID = 2, Name = "Genre2"},
                new Genre { GenreID = 3, Name = "Genre3"}
            };

            _context.Genres.AddRange(genresToAdd);
            _context.SaveChanges();
            var genres = await _genreService.GetGenres();

            Assert.NotNull(genres.First());
            Assert.Equal(genresToAdd[0].Name, genres.First().Name);
        }

        [Fact]
        public async void GetGenresFails()
        {
            var genres = await _genreService.GetGenres();

            Assert.Equal(genres.ToList(), new List<Genre>());
            Assert.NotEqual(genres.ToList(), new List<Genre>() { new Genre { Name = "LOL" } });
        }

        [Fact]
        public async void UpdateGenresWorks()
        {
            var genreVM = new GenreViewModel { Name = "Genre1" };
            var genre = await _genreService.CreateGenre(genreVM);

            var genreNewName = new GenreViewModel { Name = "Genre2" };
            var updateGenre = await _genreService.UpdateGenre(genre.GenreID, genreNewName);

            Assert.Equal(updateGenre.Name, genreNewName.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async void UpdateGenreFailsWithBadNewGenreName(string name)
        {
            var genreVM = new GenreViewModel { Name = "Genre" };
            var genre = await _genreService.CreateGenre(genreVM);

            var genreNewName = new GenreViewModel { Name = name };

            await Assert.ThrowsAsync<Exception>(() => _genreService.UpdateGenre(genre.GenreID, genreNewName));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async void UpdateGenreWithBadIDFails(int id)
        {
            await Assert.ThrowsAsync<Exception>(() => _genreService.UpdateGenre(id, new GenreViewModel { Name = "TEST" }));
        }

        [Fact]
        public async void UpdateGenreFailsWithNonExistingGenre()
        {
            await Assert.ThrowsAsync<Exception>(() => _genreService.UpdateGenre(1, new GenreViewModel { Name = "TEST" }));
        }

        [Fact]
        public async void UpdateGenreFailsWithFailModel()
        {
            var genreVM1 = new GenreViewModel { Name = "Genre" };
            var genre = await _genreService.CreateGenre(genreVM1);

            var genreVM2 = new GenreViewModel();
            await Assert.ThrowsAsync<Exception>(() => _genreService.UpdateGenre(genre.GenreID, genreVM2));
        }
        public void Dispose()
        {
            _provider.Dispose();
        }

    }
}

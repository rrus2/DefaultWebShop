using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultWebShop.Services;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DefaultWebShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IProductService _productService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAdminService _adminService;
        public AdminController(IGenreService genreService, 
            IProductService productService, 
            RoleManager<IdentityRole> roleManager,
            IAdminService adminService)
        {
            _genreService = genreService;
            _productService = productService;
            _roleManager = roleManager;
            _adminService = adminService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CreateProduct()
        {
            await LoadGenres();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                await LoadGenres();
                return View(model);
            }
            await _productService.CreateProduct(model, image);
            return View(nameof(Index));
        }
        public async Task<IActionResult> CreateUser()
        {
            await LoadRoles();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserViewModel model)
        {
            await _adminService.CreateUser(model);
            return View(nameof(Index));
        }
        public async Task<IActionResult> EditUser()
        {
            await LoadUsers();
            return View();
        }

        public async Task<IActionResult> EditUserDetails(string userid)
        {
            var user = await _adminService.GetUser(userid);
            return View(user);
        }
        private async Task LoadUsers()
        {
            var users = await _adminService.GetUsers();
            ViewBag.Users = new SelectList(users, "Id", "Email");
        }

        private async Task LoadGenres()
        {
            var genres = await _genreService.GetGenres();
            ViewBag.Genres = new SelectList(genres, "GenreID", "Name");
        }
        private async Task LoadRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
        }
    }
}
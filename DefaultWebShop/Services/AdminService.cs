using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AdminService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<ApplicationUser> CreateUser(UserViewModel model)
        {
            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                Birthdate = model.Birthdate
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new Exception("Error creating user");
            var role = await _userManager.AddToRoleAsync(user, model.Role);
            if (!role.Succeeded)
                throw new Exception("Error adding user to role");
            return user;
        }

        public async Task<UserViewModel> GetUser(string userid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userid);
            var role = await _userManager.GetRolesAsync(user);
            if (user == null)
                throw new Exception("User does not exist (id: " + userid + ")");
            var model = new UserViewModel
            {
                Birthdate = user.Birthdate,
                Email = user.Email,
                Role = role.First()
            };
            return model;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<ApplicationUser> UpdateUser(UserViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (user == null)
                throw new Exception("User not found");
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Birthdate = model.Birthdate;
            var role = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRoleAsync(user, role.ToString());
            await _userManager.AddToRoleAsync(user, model.Role);
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.Password);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}

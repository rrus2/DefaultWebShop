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
            if (model == null)
                throw new Exception("User view model can not be null");
            if (model.Email == null || model.Email == string.Empty)
                throw new Exception("Model email/name can not be null or empty");
            if (model.Role == null || model.Role == string.Empty)
                throw new Exception("User role must be submitted");
            if (model.Birthdate == null)
                throw new Exception("User must have a valid birthdate");
            if (model.Birthdate > DateTime.Now)
                throw new Exception("User can not be from the future");
            if (model.Password == null || model.Password == string.Empty)
                throw new Exception("User must provide a password");
            if (model.RepeatPassword == null || model.RepeatPassword == string.Empty)
                throw new Exception("User must repeat the password correctly");

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
            var user = await _userManager.FindByIdAsync(userid);
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
            if (model == null)
                throw new Exception("Updated model can not be null");
            if (model.Email == null || model.Email == string.Empty)
                throw new Exception("Updated model email can not be null or empty");
            if (model.Password == null || model.Password == string.Empty)
                throw new Exception("Updated model password can not be null or empty");
            if (model.RepeatPassword == null || model.RepeatPassword == string.Empty)
                throw new Exception("Updated model repeated password can not be null or empty");
            var user = await _userManager.FindByIdAsync(model.Id);
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

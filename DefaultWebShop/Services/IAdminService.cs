using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultWebShop.Services
{
    public interface IAdminService
    {
        Task<ApplicationUser> CreateUser(UserViewModel model);
        Task<IEnumerable<ApplicationUser>> GetUsers();
        Task<UserViewModel> GetUser(string userid);
    }
}

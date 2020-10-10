using DefaultWebShop.Models;
using DefaultWebShop.ViewModels;
using Microsoft.AspNetCore.Identity;
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
        Task<ApplicationUser> UpdateUser(UserViewModel model);
        Task<IdentityRole> CreateRole(IdentityRoleViewModel role);
        Task<IdentityRole> DeleteRole(IdentityRoleViewModel role);
        Task<IdentityRole> UpdateRole(IdentityRoleViewModel role);
        Task<IEnumerable<IdentityRole>> GetRoles();
        Task<IEnumerable<IdentityRole>> GetRolesPerUser(string name);
    }
}

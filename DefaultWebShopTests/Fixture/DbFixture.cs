using DefaultWebShop.Models;
using DefaultWebShop.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultWebShopTests.Fixture
{
    public class DbFixture
    {
        
        public DbFixture()
        {
            var serviceCollection = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()))
                .AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().Services.AddLogging().AddHttpContextAccessor()
                .AddScoped<IGenreService, GenreService>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IAdminService, AdminService>()
                .AddScoped<IShoppingCartService, ShoppingCartService>();

            
            Provider = serviceCollection.BuildServiceProvider();
        }
        public ServiceProvider Provider { get; set; }
    }
}

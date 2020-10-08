﻿using DefaultWebShop.Models;
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
    public class DbFixture : PageModel
    {
        
        public DbFixture()
        {
            var serviceCollection = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()))
                .AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().Services.AddLogging().AddHttpContextAccessor();

            
            Provider = serviceCollection.BuildServiceProvider();
        }
        public ServiceProvider Provider { get; set; }
    }
}

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using psytest.Areas.Identity.Data;
using psytest.Models;

[assembly: HostingStartup(typeof(psytest.Areas.Identity.IdentityHostingStartup))]
namespace psytest.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<UserContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("UserContextConnection")));

                services.AddDefaultIdentity<TestingUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<UserContext>();
            });
        }
    }
}
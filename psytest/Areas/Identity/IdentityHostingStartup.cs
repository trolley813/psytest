using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using psytest.Areas.Identity.Data;
using psytest.Models;
using System;

[assembly: HostingStartup(typeof(psytest.Areas.Identity.IdentityHostingStartup))]
namespace psytest.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {

            builder.ConfigureServices((context, services) =>
            {
                string database = context.Configuration.GetValue<String>("Database").ToLower();

                switch (database)
                {
                    case "sqlite":
                        services.AddDbContext<UserContext>(options =>
                            options.UseSqlite(
                                context.Configuration.GetConnectionString("UserContextConnection")));
                        break;
                    case "postgres":
                    case "postgresql":
                        services.AddDbContext<UserContext>(options =>
                            options.UseNpgsql(
                                context.Configuration.GetConnectionString("UserContextConnection")));
                        break;
                }

                services.AddDefaultIdentity<TestingUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<UserContext>();
            });
        }
    }
}
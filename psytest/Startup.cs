using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using psytest.Areas.Identity.Data;
using psytest.Models;
using psytest.Wizard;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace psytest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton<ITestCreationRepository, TestCreationRepository>();

            services.AddSingleton<IWizardStepProvider, TestWizardStepProvider>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            string database = Configuration.GetValue<String>("Database").ToLower();

            switch (database)
            {
                case "sqlite":
                    services.AddDbContext<TestContext>
                        (options => options.UseSqlite(Configuration.GetConnectionString("TestContextConnection")));
                    services.AddDbContext<TestResultContext>
                        (options => options.UseSqlite(Configuration.GetConnectionString("TestResultContextConnection")));
                    break;
                case "postgres":
                case "postgresql":
                    services.AddDbContext<TestContext>
                        (options => options.UseNpgsql(Configuration.GetConnectionString("TestContextConnection")));
                    services.AddDbContext<TestResultContext>
                        (options => options.UseNpgsql(Configuration.GetConnectionString("TestResultContextConnection")));
                    break;
            }


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ru-RU")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            RequestCultureProvider requestProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First();
            options.RequestCultureProviders.Remove(requestProvider);

            app.UseRequestLocalization(options);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CreateRoles(services).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<TestingUser>>();

            IdentityResult roleResult;
            // Adding Admin Role
            var roleCheck1 = await RoleManager.RoleExistsAsync("Administrator");
            var roleCheck2 = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck1)
            {
                // create the roles and seed them to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Administrator"));
            }
            if (!roleCheck2)
            {
                // create the roles and seed them to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("User"));
            }
        }
    }
}

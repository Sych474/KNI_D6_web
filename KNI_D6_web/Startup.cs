using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using KNI_D6_web.Model.Database.Initialization;
using KNI_D6_web.Model.Database.Initialization.Configuration;
using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KNI_D6_web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        private DbInitializationConfiguration dbInitializationConfiguration = new DbInitializationConfiguration();
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

            Configuration.GetSection("dbInitializationConfiguration").Bind(dbInitializationConfiguration);
            services.Configure<DbInitializationConfiguration>(options => Configuration.GetSection("dbInitializationConfiguration").Bind(options));

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            InitializeDatabase(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitializeDatabase(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            DatabaseInitializer.InitializeDatabase(dbContext, dbInitializationConfiguration);
        }
    }
}

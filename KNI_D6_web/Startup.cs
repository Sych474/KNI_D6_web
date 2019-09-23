using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database;
using KNI_D6_web.Model.Database.Initialization;
using KNI_D6_web.Model.Database.Initialization.Configuration;
using KNI_D6_web.Model.Database.Repositories;
using KNI_D6_web.Model.Database.Repositories.Implementation;
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

        private DbInitializationConfiguration DbInitializationConfiguration { get; set; } = new DbInitializationConfiguration();
        
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            Configuration.GetSection("dbInitializationConfiguration").Bind(DbInitializationConfiguration);
            services.Configure<DbInitializationConfiguration>(options => Configuration.GetSection("dbInitializationConfiguration").Bind(options));

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(opts => {
                opts.Password.RequiredLength = 5;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IAchievementsCalculator, AchievementsCalculator>();
            services.AddTransient<IAchievementsManager, AchievementsManager>();

            services.AddTransient<ISemestersRepository, SemestersRepository>();
            services.AddTransient<IParametersRepository, ParametersRepository>();
            services.AddTransient<IParameterValuesRepository, ParameterValuesRepository>();
            services.AddTransient<IAchievementsRepository, AchievementsRepository>();
            services.AddTransient<IAchievementsGroupsRepository, AchievementsGroupsRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

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

            ApplyMigrations(app);
            InitializeDatabase(app).Wait();

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

        private static void ApplyMigrations(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        private async Task InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var parametersRepository = serviceScope.ServiceProvider.GetService<IParametersRepository>();
                var parameterValuesRepository = serviceScope.ServiceProvider.GetService<IParameterValuesRepository>();
                using (var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>())
                using (var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>())
                {
                    var initializer = new DatabaseInitializer(userManager, roleManager, parametersRepository, parameterValuesRepository);
                    await initializer.InitializeDatabase(DbInitializationConfiguration);
                }
            }
        }
    }
}

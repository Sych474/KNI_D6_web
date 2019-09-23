using KNI_D6_web.Model.Database.Initialization.Configuration;
using KNI_D6_web.Model.Database.Repositories;
using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Initialization
{
    public class DatabaseInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IParametersRepository parametersRepository;
        private readonly IParameterValuesRepository parameterValuesRepository;

        public DatabaseInitializer(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IParametersRepository parametersRepository, IParameterValuesRepository parameterValuesRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.parametersRepository = parametersRepository;
            this.parameterValuesRepository = parameterValuesRepository;
        }

        public async Task InitializeDatabase(DbInitializationConfiguration configuration)
        {
            await AddUserRoles(roleManager);

            await AddParameters(configuration.Parameters);

            await AddAdminUser(configuration.AdminLogin, configuration.AdminPassword, configuration.AdminEmail);
        }

        private async Task AddAdminUser(string adminLogin, string adminPassword, string adminEmail)
        {
            var admin = await userManager.FindByNameAsync(adminLogin);
            if (admin == null)
            {
                admin = new User()
                {
                    UserName = adminLogin,
                    Email = adminEmail
                };
                var identityResult = await userManager.CreateAsync(admin, adminPassword);

                if (!identityResult.Succeeded)
                    throw new Exception("Can not create admin user");

                var createdAdmin = await userManager.FindByNameAsync(adminLogin);

                await parameterValuesRepository.AddAllParameterValuesForUserAsync(createdAdmin.Id);
            }

            foreach (var role in UserRoles.Roles)
            {
                if (!await userManager.IsInRoleAsync(admin, role))
                    await userManager.AddToRoleAsync(admin, role);
            }
        }

        private async Task AddUserRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var item in UserRoles.Roles)
            {
                if (!await roleManager.RoleExistsAsync(item))
                    await roleManager.CreateAsync(new IdentityRole(item));
            }
        }

        private async Task AddParameters(IEnumerable<string> parameterNames)
        {
            foreach (var name in parameterNames)
            {
                var currParameter = await parametersRepository.FindParameterByNameAsync(name);
                if (currParameter == null)
                {
                    try
                    {
                        await parametersRepository.AddParameterAsync(new Parameter() { Name = name });
                    }
                    catch (Exception ex)
                    {
                        //TO_DO log errors
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}

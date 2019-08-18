using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database.Initialization.Configuration;
using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Initialization
{
    public class DatabaseInitializer
    {
        private static readonly int DefaultValue = 0;

        public static async Task InitializeDatabase(ApplicationDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, DbInitializationConfiguration configuration)
        {
            await AddUserRoles(roleManager);

            await AddAdminUser(userManager, configuration.AdminLogin, configuration.AdminPassword, configuration.AdminEmail);

            await AddParameters(dbContext, configuration.Parameters);

            await AddParameterValues(dbContext, configuration.Parameters, configuration.ParameterValuesPresets);
        }

        private static async Task AddAdminUser(UserManager<User> userManager, string adminLogin, string adminPassword, string adminEmail)
        {
            var admin = await userManager.FindByNameAsync(adminLogin);
            if (admin == null)
            {
                admin = new User() { UserName = adminLogin, Email = adminEmail };
                var identityResult = await userManager.CreateAsync(admin, adminPassword);
                if (!identityResult.Succeeded)
                    throw new Exception("Can not create admin user");
            }

            foreach (var role in UserRoles.Roles)
            {
                if (!await userManager.IsInRoleAsync(admin, role))
                    await userManager.AddToRoleAsync(admin, role);
            }
        }

        private static async Task AddUserRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var item in UserRoles.Roles)
            {
                if (!await roleManager.RoleExistsAsync(item))
                    await roleManager.CreateAsync(new IdentityRole(item));
            }
        }

        private static async Task AddParameters(ApplicationDbContext dbContext, IEnumerable<string> parameterNames)
        {
            foreach (var name in parameterNames)
            {
                if (!await dbContext.Parameters.AnyAsync(x => x.Name == name))
                {
                    await dbContext.Parameters.AddAsync(new Parameter() { Name = name });
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private static async Task AddParameterValues(ApplicationDbContext dbContext, IEnumerable<string> parameterNames, IEnumerable<ParameterValuesPreset> parameterValuesPresets)
        {
            foreach (var item in parameterValuesPresets)
            {
                var parameter = dbContext.Parameters.AsNoTracking().Where(p => p.Name == item.ParameterName).FirstOrDefault();
                var user = dbContext.Users.AsNoTracking().Where(u => u.UserName == item.UserLogin).FirstOrDefault();
                if (parameter != null && user != null)
                {
                    var parameterValue = dbContext.ParameterValues.Where(x => x.UserId == user.Id && x.ParameterId == parameter.Id).FirstOrDefault();
                    if (parameterValue == null)
                        await dbContext.ParameterValues.AddAsync(new ParameterValue() { UserId = user.Id, ParameterId = parameter.Id, Value = item.Value });
                    else if (parameterValue.Value == DefaultValue)
                        parameterValue.Value = item.Value;                        
                    await dbContext.SaveChangesAsync();
                }
            }

            //add Default values
            foreach (var user in dbContext.Users.Include(u=>u.ParameterValues).ThenInclude(pv => pv.Parameter))
            {
                foreach (var name in parameterNames)
                {
                    var parameter = dbContext.Parameters.AsNoTracking().Where(p => p.Name == name).FirstOrDefault();
                    if (parameter != null)
                    {
                        if (!await dbContext.ParameterValues.AnyAsync(x => x.UserId == user.Id && x.ParameterId == parameter.Id))
                        {
                            await dbContext.ParameterValues.AddAsync(new ParameterValue() { UserId = user.Id, ParameterId = parameter.Id, Value = DefaultValue });
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}

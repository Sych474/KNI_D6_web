using KNI_D6_web.Model.Database.Initialization.Configuration;
using KNI_D6_web.Model.Parameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KNI_D6_web.Model.Database.Initialization
{
    public class DatabaseInitializer
    {
        private static readonly int DefaultValue = 0;


        public static void InitializeDatabase(ApplicationDbContext dbContext, DbInitializationConfiguration configuration)
        {
            AddParameters(dbContext, configuration.Parameters);

            AddParameterValues(dbContext, configuration.Parameters, configuration.ParameterValuesPresets);

            AddAchievements(dbContext, configuration.Achievements);

            AddUserAchievements(dbContext, configuration.UserAchievementsPresets);
        }

        private static void AddUserAchievements(ApplicationDbContext dbContext, IEnumerable<UserAchievementsPreset> userAchievementsPresets)
        {
            foreach (var item in userAchievementsPresets)
            {
                var achievement = dbContext.Achievements.Where(p => p.Name == item.AchievementName).FirstOrDefault();
                var user = dbContext.Users.Where(u => u.UserName == item.UserLogin).FirstOrDefault();
                if (achievement != null && user != null)
                    if (!dbContext.UserAchievements.Any(x => x.AchievementId == achievement.Id && x.UserId == user.Id))
                        dbContext.UserAchievements.Add(new UserAchievement() { UserId = user.Id, AchievementId = achievement.Id });
            }
            dbContext.SaveChanges();
        }

        private static void AddAchievements(ApplicationDbContext dbContext, IEnumerable<string> achievementNames)
        {
            foreach (var name in achievementNames)
            {
                if (!dbContext.Achievements.Any(x => x.Name == name))
                    dbContext.Achievements.Add(new Achievement() { Name = name });
            }
            dbContext.SaveChanges();
        }

        private static void AddParameters(ApplicationDbContext dbContext, IEnumerable<string> parameterNames)
        {
            foreach (var name in parameterNames)
            {
                if (!dbContext.Parameters.Any(x => x.Name == name))
                    dbContext.Parameters.Add(new Parameter() { Name = name });
            }
            dbContext.SaveChanges();
        }

        private static void AddParameterValues(ApplicationDbContext dbContext, IEnumerable<string> parameterNames, IEnumerable<ParameterValuesPreset> parameterValuesPresets)
        {
            foreach (var item in parameterValuesPresets)
            {
                var parameter = dbContext.Parameters.Where(p => p.Name == item.ParameterName).FirstOrDefault();
                var user = dbContext.Users.Where(u => u.UserName == item.UserLogin).FirstOrDefault();
                if (parameter != null && user != null)
                {
                    var parameterValue = dbContext.ParameterValues.Where(x => x.UserId == user.Id && x.ParameterId == parameter.Id).FirstOrDefault();
                    if (parameterValue == null)
                        dbContext.ParameterValues.Add(new ParameterValue() { UserId = user.Id, ParameterId = parameter.Id, Value = item.Value });
                    else if (parameterValue.Value == DefaultValue)
                        parameterValue.Value = item.Value;                        
                }
            }

            //add Default values
            foreach (var user in dbContext.Users.Include(u=>u.ParameterValues).ThenInclude(pv => pv.Parameter))
            {
                foreach (var name in parameterNames)
                {
                    var parameter = dbContext.Parameters.Where(p => p.Name == name).FirstOrDefault();
                    if (parameter != null)
                        if (!dbContext.ParameterValues.Any(x => x.UserId == user.Id && x.ParameterId == parameter.Id))
                            dbContext.ParameterValues.Add(new ParameterValue() { UserId = user.Id, ParameterId = parameter.Id, Value = DefaultValue });
                }
            }
            dbContext.SaveChanges();
        }

        private static void AddParameterValueIfNotExists(ApplicationDbContext dbContext, ParameterValue parameterValue)
        {
            if (!dbContext.ParameterValues.Any(x => x.UserId == parameterValue.UserId && x.ParameterId == parameterValue.ParameterId))
                dbContext.ParameterValues.Add(parameterValue);
            dbContext.SaveChanges();
        }
    }
}

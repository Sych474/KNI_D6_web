using KNI_D6_web.Model;
using KNI_D6_web.Model.Database.Repositories;
using KNI_D6_web.ViewModels.Components.AchievementsProgress;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Components
{
    public class AchievementsProgressViewComponent : ViewComponent
    {
        private readonly ISemestersRepository semestersRepository;
        private readonly IAchievementsGroupsRepository achievementsGroupsRepository;
        private readonly UserManager<User> userManager;

        public AchievementsProgressViewComponent(ISemestersRepository semestersRepository, IAchievementsGroupsRepository achievementsGroupsRepository, UserManager<User> userManager)
        {
            this.semestersRepository = semestersRepository;
            this.achievementsGroupsRepository = achievementsGroupsRepository;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string login)
        {
            var viewModel = new AchievementsProgressViewModel()
            {
                AchievementsProgressInGroups = await GetAchievementsProgressInGroups(login)
            };
            return View(viewModel);
        }

        private async Task<IEnumerable<IOrderedEnumerable<AchievementProgressViewModel>>> GetAchievementsProgressInGroups(string login)
        {
            var user = await userManager.Users
                .Include(u => u.ParameterValues)
                .Include(u => u.UserAchievements)
                .FirstOrDefaultAsync(u => u.UserName == login);

            var groups = await achievementsGroupsRepository.GetAchievementsGroupsAsync();

            var semester = await semestersRepository.FindCurrentSemesterAsync();

            var progressGroups = new List<IOrderedEnumerable<AchievementProgressViewModel>>();

            foreach (var group in groups.Where(g => g.Ahievements.Any()))
            {
                var progressGroup = new List<AchievementProgressViewModel>();

                var filteredAchievements = semester == null ? 
                    group.Ahievements : 
                    group.Ahievements.Where(
                        a => a.SemesterId == null || 
                        a.SemesterId == semester.Id ||
                        user.UserAchievements.Any(ua => ua.AchievementId == a.Id)
                        );

                foreach (var achievement in filteredAchievements)
                {
                    var linkedParameterValue = user.ParameterValues.Where(pv => pv.ParameterId == achievement.ParameterId).FirstOrDefault();

                    var progress = new AchievementProgressViewModel()
                    {
                        AchievementId = achievement.Id,
                        AchievementName = achievement.Name,
                        AchievementDescription = achievement.Description,
                        NumberInGroup = achievement.NumberInGroup,
                        AchievementValue = achievement.AchievementValue,
                        LinkedParameterValue = linkedParameterValue?.Value,
                        IsReceived = user.UserAchievements.Any(ua => ua.AchievementId == achievement.Id),
                        AchievementType = achievement.AchievementType
                    };
                    progressGroup.Add(progress);
                }
                progressGroups.Add(progressGroup.OrderBy(ap => ap.NumberInGroup));
            }

            return progressGroups.Where(g => g.Any());
        }
    }
}

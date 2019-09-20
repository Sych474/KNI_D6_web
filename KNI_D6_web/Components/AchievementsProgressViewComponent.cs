using KNI_D6_web.Model.Database;
using KNI_D6_web.Model.Database.Repositories;
using KNI_D6_web.ViewModels.Components.AchievementsProgress;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Components
{
    public class AchievementsProgressViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISemestersRepository semestersRepository;

        public AchievementsProgressViewComponent(ApplicationDbContext dbContext, ISemestersRepository semestersRepository)
        {
            this.dbContext = dbContext;
            this.semestersRepository = semestersRepository;
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
            var user = await dbContext.Users
                .Include(u => u.ParameterValues)
                .Include(u => u.UserAchievements)
                .FirstOrDefaultAsync(u => u.UserName == login);

            var groups = await dbContext?.AchievementGroups?
                .Include(ag => ag.Ahievements)?
                .ThenInclude(a => a.AchievementParameters)?
                .ThenInclude(ap => ap.Parameter)?
                .ToListAsync();

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
                    var linkedParameter = achievement.AchievementParameters.FirstOrDefault()?.Parameter;
                    var linkedParameterValue = user.ParameterValues.Where(pv => pv.ParameterId == linkedParameter?.Id).FirstOrDefault();

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

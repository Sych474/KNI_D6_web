using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database;
using KNI_D6_web.ViewModels.Achievements;
using KNI_D6_web.ViewModels.AchievementsProgress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Controllers
{
    [Authorize]
    public class AchievementsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAchievementsManager achievementsManager;
        private readonly UserManager<User> userManager;

        public AchievementsController(ApplicationDbContext dbContext, IAchievementsManager achievementsManager, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.achievementsManager = achievementsManager;
            this.userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IActionResult result;
            if (User.Identity.IsAuthenticated)
            {
                result = RedirectToAction(nameof(Progress), new { login = User.Identity.Name });
            }
            else
            {
                var viewModel = new AchievementsViewModel()
                {
                    AchievementsInGroups = await GetAchievementsInGroups()
                };
                result = View(viewModel);                
            }
            return result;
        }

        public async Task<IActionResult> Progress(string login)
        {
            var viewModel = new AchievementsProgressViewModel()
            {
                AchievementsProgressInGroups = await GetAchievementsProgressInGroups(login)
            };
            return View(viewModel);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult CreateCustomAchievement()
        {
            var viewModel = new CreateCustomAchievementViewModel()
            {
                AchievementGroups = CreateAchievementGroupsList(null)
            };

            return View(viewModel);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomAchievement
            ([Bind("AchievementName, AhievementDescription, NumberInGroup, GroupId")] CreateCustomAchievementViewModel viewModel)
        {
            viewModel.AchievementGroups = CreateAchievementGroupsList(viewModel.GroupId);
            IActionResult result = View(viewModel);

            if (ModelState.IsValid)
            {
                var id = await achievementsManager.AddCustomAchievement(
                    viewModel.AchievementName, 
                    viewModel.AhievementDescription,
                    viewModel.GroupId,
                    viewModel.NumberInGroup);
                if (id.HasValue)  
                    result = RedirectToAction(nameof(Index));
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult CreateValueAchievement()
        {
            var viewModel = new CreateValueAchievementViewModel()
            {
                Parameters = CreateParametersList(null),
                AchievementGroups = CreateAchievementGroupsList(null)

            };
            return View(viewModel);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateValueAchievement(
            [Bind("AchievementName, AhievementDescription, AchievementValue, ParameterId, NumberInGroup, GroupId")] CreateValueAchievementViewModel viewModel)
        {
            viewModel.AchievementGroups = CreateAchievementGroupsList(viewModel.GroupId);
            viewModel.Parameters = CreateParametersList(viewModel.ParameterId);

            IActionResult result = View(viewModel);
            if (ModelState.IsValid)
            {
                var id = await achievementsManager.AddCalculatedAchievement(
                    viewModel.AchievementName, 
                    viewModel.AhievementDescription, 
                    viewModel.AchievementValue, 
                    viewModel.ParameterId,
                    viewModel.GroupId,
                    viewModel.NumberInGroup
                    );
                if (id.HasValue)
                {
                    await achievementsManager.CheckAndUpdateСalculatedAchievementForUsers(dbContext.Users.Select(u => u.Id), id.Value);
                    result = RedirectToAction(nameof(Index));
                }
            }
            return result;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await dbContext.Achievements
                .Include(a => a.AchievementGroup)
                .Include(a => a.AchievementParameters).ThenInclude(ap => ap.Parameter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (achievement == null)
            {
                return NotFound();
            }

            return View(achievement);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult result = NotFound();
            if (id != null)
            {
                var achievement = await dbContext.Achievements.FindAsync(id);
                if (achievement != null)
                {
                    var achievementParameter = await dbContext.AchievementParameters.FirstOrDefaultAsync(ap => ap.AchievementId == id);
                    var viewModel = new EditAchievementViewModel(achievement, achievementParameter?.ParameterId);
                    ViewData["AchievementsGroupId"] = CreateAchievementGroupsList(achievement.AchievementsGroupId);
                    ViewData["Parameters"] = CreateParametersList(achievementParameter?.ParameterId);
                    result =  View(viewModel);
                }        
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AchievementsGroupId,NumberInGroup,Name,Description,AchievementType,AchievementValue,AchievementParameterId")] EditAchievementViewModel viewModel)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                if (id == viewModel.Id)
                {
                    try
                    {
                        var achievement = await dbContext.Achievements.FindAsync(id);
                        var oldAchievementParameter = await dbContext.AchievementParameters.FirstOrDefaultAsync(ap => ap.AchievementId == id);

                        var newAchievementParameter = viewModel.GetAchievementParameter();
                        viewModel.UpdateAchievement(achievement);

                        dbContext.Update(achievement);

                        if (oldAchievementParameter != null)
                            dbContext.AchievementParameters.Remove(oldAchievementParameter);
                        if (newAchievementParameter != null)
                            dbContext.AchievementParameters.Add(newAchievementParameter);
                        await dbContext.SaveChangesAsync();
                        ViewData["AchievementsGroupId"] = CreateAchievementGroupsList(achievement.AchievementsGroupId);
                        ViewData["Parameters"] = CreateParametersList(newAchievementParameter?.ParameterId);

                        await achievementsManager.CheckAndUpdateСalculatedAchievementForUsers(dbContext.Users.Select(u => u.Id), id);
                        result = RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (AchievementExists(id))
                        {
                            throw;
                        }
                    }
                }
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await dbContext.Achievements
                .Include(a => a.AchievementGroup)
                .Include(a => a.AchievementParameters).ThenInclude(ap => ap.Parameter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (achievement == null)
            {
                return NotFound();
            }

            return View(achievement);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var achievement = await dbContext.Achievements.Include(a => a.AchievementParameters).FirstOrDefaultAsync(a => a.Id == id);

            dbContext.AchievementParameters.RemoveRange(achievement.AchievementParameters);
            dbContext.Achievements.Remove(achievement);

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AchievementExists(int id)
        {
            return dbContext.Achievements.Any(e => e.Id == id);
        }

        private SelectList CreateParametersList(int? currentId)
        {
            return new SelectList(dbContext.Parameters, "Id","Name", currentId);
        }
        
        private SelectList CreateAchievementGroupsList(int? currentId)
        {
            return new SelectList(dbContext.AchievementGroups, "Id", "Name", currentId);
        }

        private async Task<IEnumerable<IOrderedEnumerable<Achievement>>> GetAchievementsInGroups()
        {
            var groups = await dbContext?.AchievementGroups?.Include(ag => ag.Ahievements)?.ToListAsync();

            return groups?.Select(g => g.Ahievements.OrderBy(a => a.NumberInGroup));
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

            var progressGroups = new List<IOrderedEnumerable<AchievementProgressViewModel>>();

            foreach (var group in groups)
            {
                var progressGroup = new List<AchievementProgressViewModel>();
                foreach (var achievement in group.Ahievements)
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
                        LinkedParameterName = linkedParameter?.Name,
                        LinkedParameterValue = linkedParameterValue?.Value,
                        IsReceived = user.UserAchievements.Any(ua => ua.AchievementId == achievement.Id)
                    };
                    progressGroup.Add(progress);
                }
                progressGroups.Add(progressGroup.OrderBy(ap => ap.NumberInGroup));
            }

            return progressGroups;
        }
    }
}
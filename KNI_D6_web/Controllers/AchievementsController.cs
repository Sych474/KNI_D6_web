using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database;
using KNI_D6_web.Model.Database.Repositories;
using KNI_D6_web.ViewModels.Achievements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AchievementsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAchievementsManager achievementsManager;
        private readonly ISemestersRepository semestersRepository;

        public AchievementsController(ApplicationDbContext dbContext, IAchievementsManager achievementsManager, ISemestersRepository semestersRepository)
        {
            this.dbContext = dbContext;
            this.achievementsManager = achievementsManager;
            this.semestersRepository = semestersRepository;
        }

        [AllowAnonymous]
        [HttpGet, ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new AchievementsViewModel()
            {
                AchievementsInGroups = await GetAchievementsInGroups()
            };
            return View(viewModel);                
        }

        [HttpGet, Route("Progress")]
        public IActionResult Progress(string userLogin)
        {
            return View(new AchievementsProgressViewModel() { Login = userLogin });
        }

        [HttpGet, Route("All")]
        public async Task<IActionResult> All()
        {
            var viewModel = new AchievementsViewModel()
            {
                AchievementsInGroups = await GetAllAchievementsInGroups()
            };
            return View(viewModel);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet, Route("CreateCustomAchievement")]
        public IActionResult CreateCustomAchievement()
        {
            ViewData["AchievementGroups"] = CreateAchievementGroupsList(null);
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(null);

            return View(new CreateCustomAchievementViewModel());
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, Route("CreateCustomAchievement")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomAchievement
            ([Bind("AchievementName, AhievementDescription, NumberInGroup, GroupId, SemesterId")] CreateCustomAchievementViewModel viewModel)
        {
            ViewData["AchievementGroups"] = CreateAchievementGroupsList(viewModel.GroupId);
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(viewModel.SemesterId);

            IActionResult result = View(viewModel);

            if (ModelState.IsValid)
            {
                var id = await achievementsManager.AddCustomAchievement(
                    viewModel.AchievementName, 
                    viewModel.AhievementDescription,
                    viewModel.GroupId,
                    viewModel.NumberInGroup,
                    viewModel.SemesterId
                    );
                if (id.HasValue)  
                    result = RedirectToAction(nameof(All));
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet, Route("CreateValueAchievement")]
        public IActionResult CreateValueAchievement()
        {
            ViewData["Parameters"] = CreateParametersList(null);
            ViewData["AchievementGroups"] = CreateAchievementGroupsList(null);
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(null);
            
            return View(new CreateValueAchievementViewModel());
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, Route("CreateValueAchievement")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateValueAchievement(
            [Bind("AchievementName, AhievementDescription, AchievementValue, ParameterId, NumberInGroup, GroupId, SemesterId")] CreateValueAchievementViewModel viewModel)
        {
            ViewData["Parameters"] = CreateParametersList(viewModel.ParameterId);
            ViewData["AchievementGroups"] = CreateAchievementGroupsList(viewModel.GroupId);
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(viewModel.SemesterId);

            IActionResult result = View(viewModel);
            if (ModelState.IsValid)
            {
                var id = await achievementsManager.AddCalculatedAchievement(
                    viewModel.AchievementName, 
                    viewModel.AhievementDescription, 
                    viewModel.AchievementValue, 
                    viewModel.ParameterId,
                    viewModel.GroupId,
                    viewModel.NumberInGroup,
                    viewModel.SemesterId
                    );
                if (id.HasValue)
                {
                    await achievementsManager.CheckAndUpdateСalculatedAchievementForUsers(dbContext.Users.Select(u => u.Id), id.Value);
                    result = RedirectToAction(nameof(All));
                }
            }
            return result;
        }

        [AllowAnonymous]
        [HttpGet, Route("Details")]
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
        [HttpGet, Route("EditCustom")]
        public async Task<IActionResult> EditCustom(int id)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                var achievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Id == id);
                if (achievement != null)
                {
                    var achievementParameter = await dbContext.AchievementParameters.FirstOrDefaultAsync(ap => ap.AchievementId == id);
                    var viewModel = new EditAchievementViewModel(achievement, achievementParameter?.ParameterId);
                    ViewData["AchievementsGroupId"] = CreateAchievementGroupsList(achievement.AchievementsGroupId);
                    ViewData["SemestersSelectList"] = CreateSemestersSelectList(achievement.SemesterId);
                    result =  View(viewModel);
                }        
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet, Route("EditCalculated")]
        public async Task<IActionResult> EditCalculated(int id)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                var achievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Id == id);
                if (achievement != null)
                {
                    var achievementParameter = await dbContext.AchievementParameters.FirstOrDefaultAsync(ap => ap.AchievementId == id);
                    var viewModel = new EditAchievementViewModel(achievement, achievementParameter?.ParameterId);
                    ViewData["AchievementsGroupId"] = CreateAchievementGroupsList(achievement.AchievementsGroupId);
                    ViewData["Parameters"] = CreateParametersList(achievementParameter?.ParameterId);
                    ViewData["SemestersSelectList"] = CreateSemestersSelectList(achievement.SemesterId);
                    result = View(viewModel);
                }
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, Route("EditCustom")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustom(int id, [Bind("Id,AchievementsGroupId,NumberInGroup,Name,Description,AchievementType,SemesterId")] EditAchievementViewModel viewModel)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                if (id == viewModel.Id)
                {
                    try
                    {
                        var achievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Id == id);
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
                        ViewData["SemestersSelectList"] = CreateSemestersSelectList(achievement.SemesterId);
                        result = RedirectToAction(nameof(All));
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
        [HttpPost, Route("EditCalculated")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCalculated(int id, [Bind("Id,AchievementsGroupId,NumberInGroup,Name,Description,AchievementType,AchievementValue,AchievementParameterId,SemesterId")] EditAchievementViewModel viewModel)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                if (id == viewModel.Id)
                {
                    try
                    {
                        var achievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Id == id);
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
                        ViewData["SemestersSelectList"] = CreateSemestersSelectList(achievement.SemesterId);
                        await achievementsManager.CheckAndUpdateСalculatedAchievementForUsers(dbContext.Users.Select(u => u.Id), id);
                        result = RedirectToAction(nameof(All));
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
        [HttpGet, Route("Delete")]
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
            return RedirectToAction(nameof(All));
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

        private SelectList CreateSemestersSelectList(int? currentId)
        {
            var tmpList = dbContext.Semesters.Select(s => new CustomSelectListItem() { Name = s.Name, Id = s.Id }).ToList();
            tmpList.Insert(0, new CustomSelectListItem() { Name = "Без семестра", Id = null });
            return new SelectList(tmpList, "Id", "Name", currentId);
        }

        private async Task<IEnumerable<IOrderedEnumerable<Achievement>>> GetAchievementsInGroups()
        {
            var groups = await dbContext?.AchievementGroups?.Include(ag => ag.Ahievements)?.ToListAsync();
            var currentSemester = await semestersRepository.FindCurrentSemesterAsync();
            IEnumerable<IOrderedEnumerable<Achievement>> result;
            if (currentSemester != null)
                result = groups?.Select(g => g.Ahievements.Where(a => a.SemesterId == null || a.SemesterId == currentSemester.Id).OrderBy(a => a.NumberInGroup));
            else
                result = groups?.Select(g => g.Ahievements.OrderBy(a => a.NumberInGroup));

            return result.Where(g => g.Any());
        }

        private async Task<IEnumerable<IOrderedEnumerable<Achievement>>> GetAllAchievementsInGroups()
        {
            var groups = await dbContext?.AchievementGroups?.Include(ag => ag.Ahievements)?.ToListAsync();

            return groups?.Select(g => g.Ahievements.OrderBy(a => a.NumberInGroup));
        }
    }
}
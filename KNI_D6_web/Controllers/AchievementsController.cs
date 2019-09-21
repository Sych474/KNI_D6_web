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
        [HttpGet, Route("Create")]
        public IActionResult Create()
        {
            ViewData["Parameters"] = CreateParametersList(null);
            ViewData["AchievementGroups"] = CreateAchievementGroupsList(null);
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(null);

            return View(new CreateValueAchievementViewModel());
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("AchievementName, AhievementDescription, AchievementValue, ParameterId, NumberInGroup, GroupId, SemesterId, AchievementType")] CreateValueAchievementViewModel viewModel)
        {
            ViewData["Parameters"] = CreateParametersList(viewModel.ParameterId);
            ViewData["AchievementGroups"] = CreateAchievementGroupsList(viewModel.GroupId);
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(viewModel.SemesterId);

            IActionResult result = View(viewModel);
            if (ModelState.IsValid)
            {
                var id = await achievementsManager.AddAchievement(new Achievement()
                {
                    Name = viewModel.AchievementName, 
                    Description = viewModel.AhievementDescription, 
                    AchievementValue = viewModel.AchievementValue, 
                    ParameterId = viewModel.ParameterId,
                    AchievementsGroupId = viewModel.GroupId,
                    NumberInGroup = viewModel.NumberInGroup,
                    SemesterId = viewModel.SemesterId,
                    AchievementType = viewModel.AchievementType
                });
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
                .Include(a => a.Parameter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (achievement == null)
            {
                return NotFound();
            }

            return View(achievement);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet, Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                var achievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Id == id);
                if (achievement != null)
                {
                    var viewModel = new EditAchievementViewModel(achievement);
                    ViewData["AchievementsGroupId"] = CreateAchievementGroupsList(achievement.AchievementsGroupId);
                    ViewData["Parameters"] = CreateParametersList(achievement.ParameterId);
                    ViewData["SemestersSelectList"] = CreateSemestersSelectList(achievement.SemesterId);
                    result = View(viewModel);
                }
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AchievementsGroupId,NumberInGroup,Name,Description,AchievementType,AchievementValue,ParameterId,SemesterId")] EditAchievementViewModel viewModel)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                if (id == viewModel.Id)
                {
                    try
                    {
                        var achievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Id == id);
                        
                        viewModel.UpdateAchievement(achievement);
                        dbContext.Update(achievement);
                        await dbContext.SaveChangesAsync();

                        ViewData["AchievementsGroupId"] = CreateAchievementGroupsList(viewModel.AchievementsGroupId);
                        ViewData["Parameters"] = CreateParametersList(viewModel.ParameterId);
                        ViewData["SemestersSelectList"] = CreateSemestersSelectList(viewModel.SemesterId);
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
                .Include(a => a.Parameter)
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
            var achievement = await dbContext.Achievements.FirstOrDefaultAsync(a => a.Id == id);
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
            var tmpList = dbContext.Parameters.Select(p => new CustomSelectListItem() { Name = p.Name, Id = p.Id }).ToList();
            tmpList.Insert(0, new CustomSelectListItem() { Name = "Без параметра", Id = null });
            return new SelectList(tmpList, "Id", "Name", currentId);
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
            var groups = await dbContext?.AchievementGroups?.Include(ag => ag.Ahievements)?.ThenInclude(a => a.Parameter)?.ToListAsync();

            return groups?.Select(g => g.Ahievements.OrderBy(a => a.NumberInGroup));
        }
    }
}
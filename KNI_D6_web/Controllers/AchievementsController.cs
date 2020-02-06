using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database.Repositories;
using KNI_D6_web.ViewModels.Achievements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KNI_D6_web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AchievementsController : Controller
    {
        private readonly IAchievementsManager achievementsManager;
        private readonly ISemestersRepository semestersRepository;
        private readonly IParametersRepository parametersRepository;
        private readonly IAchievementsRepository achievementsRepository;
        private readonly IAchievementsGroupsRepository achievementsGroupsRepository;
        private readonly UserManager<User> userManager;

        public AchievementsController(IAchievementsManager achievementsManager, ISemestersRepository semestersRepository, IParametersRepository parametersRepository, IAchievementsRepository achievementsRepository, IAchievementsGroupsRepository achievementsGroupsRepository, UserManager<User> userManager)
        {
            this.achievementsManager = achievementsManager;
            this.semestersRepository = semestersRepository;
            this.parametersRepository = parametersRepository;
            this.achievementsRepository = achievementsRepository;
            this.achievementsGroupsRepository = achievementsGroupsRepository;
            this.userManager = userManager;
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

        [AllowAnonymous]
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
        public async Task<IActionResult> Create()
        {
            ViewData["Parameters"] = await CreateParametersList(null);
            ViewData["AchievementGroups"] = await CreateAchievementGroupsList(null);
            ViewData["SemestersSelectList"] = await CreateSemestersSelectList(null);

            return View(new CreateAchievementViewModel());
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("AchievementName, AhievementDescription, AchievementValue, ParameterId, NumberInGroup, GroupId, SemesterId, AchievementType")] CreateAchievementViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Parameters"] = await CreateParametersList(viewModel.ParameterId);
                ViewData["AchievementGroups"] = await CreateAchievementGroupsList(viewModel.GroupId);
                ViewData["SemestersSelectList"] = await CreateSemestersSelectList(viewModel.SemesterId);
                return View(viewModel);
            }

            var achievement = new Achievement()
            {
                Name = viewModel.AchievementName,
                Description = viewModel.AhievementDescription,
                AchievementValue = viewModel.AchievementValue,
                ParameterId = viewModel.ParameterId,
                AchievementsGroupId = viewModel.GroupId,
                NumberInGroup = viewModel.NumberInGroup,
                SemesterId = viewModel.SemesterId,
                AchievementType = viewModel.AchievementType
            };

            IActionResult result;
            try
            {
                int id = await achievementsRepository.AddAchievementAsync(achievement);
                await achievementsManager.CheckAndUpdateСalculatedAchievementForUsers(userManager.Users.Select(u => u.Id), id);
                result = RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }            
            return result;
        }

        [AllowAnonymous]
        [HttpGet, Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();
            
            var achievement = await achievementsRepository.FindAchievementByIdAsync(id.Value);
            if (achievement == null)
                return NotFound($"Achievement with id {id} not found");

            return View(achievement);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet, Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
       
            var achievement = await achievementsRepository.FindAchievementByIdAsync(id);
            if (achievement == null)
                return NotFound($"Achievement with id {id} not found");

            
            var viewModel = new EditAchievementViewModel(achievement);
            ViewData["AchievementsGroupId"] = await CreateAchievementGroupsList(achievement.AchievementsGroupId);
            ViewData["Parameters"] = await CreateParametersList(achievement.ParameterId);
            ViewData["SemestersSelectList"] = await CreateSemestersSelectList(achievement.SemesterId);
            return View(viewModel);            
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("Id,AchievementsGroupId,NumberInGroup,Name,Description,AchievementType,AchievementValue,ParameterId,SemesterId")] EditAchievementViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AchievementsGroupId"] = await CreateAchievementGroupsList(viewModel.AchievementsGroupId);
                ViewData["Parameters"] = await CreateParametersList(viewModel.ParameterId);
                ViewData["SemestersSelectList"] = await CreateSemestersSelectList(viewModel.SemesterId);
                return View(viewModel);
            }

            if (id != viewModel.Id)
                return BadRequest();

            var achievement = await achievementsRepository.FindAchievementByIdAsync(id);
            if (achievement == null)
                return NotFound($"Achievement with Id {id} not found");

            IActionResult result;
            try
            {        
                viewModel.UpdateAchievement(achievement);

                await achievementsRepository.UpdateAchievementAsync(achievement);
                
                await achievementsManager.CheckAndUpdateСalculatedAchievementForUsers(userManager.Users.Select(u => u.Id), id);
                result = RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet, Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var achievement = await achievementsRepository.FindAchievementByIdAsync(id.Value);
            if (achievement == null)
                return NotFound($"Achievement with Id {id} not found");

            return View(achievement);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await achievementsRepository.RemoveAchievementByIdAsync(id);
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        private async Task<SelectList> CreateParametersList(int? currentId)
        {
            var tmpList = (await parametersRepository.GetParametersAsync())
                .Select(p => new CustomSelectListItem() { Name = p.Name, Id = p.Id }).ToList();
            tmpList.Insert(0, new CustomSelectListItem() { Name = "Без параметра", Id = null });
            return new SelectList(tmpList, "Id", "Name", currentId);
        }
        
        private async Task<SelectList> CreateAchievementGroupsList(int? currentId)
        {
            return new SelectList(await achievementsGroupsRepository.GetAchievementsGroupsAsync(), "Id", "Name", currentId);
        }

        private async Task<SelectList> CreateSemestersSelectList(int? currentId)
        {
            var tmpList = (await semestersRepository.GetSemestersAsync())
                .Select(s => new CustomSelectListItem() { Name = s.Name, Id = s.Id }).ToList();
            tmpList.Insert(0, new CustomSelectListItem() { Name = "Без семестра", Id = null });
            return new SelectList(tmpList, "Id", "Name", currentId);
        }

        private async Task<IEnumerable<IOrderedEnumerable<Achievement>>> GetAchievementsInGroups()
        {
            var groups = await achievementsGroupsRepository.GetAchievementsGroupsAsync();
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
            var groups = await achievementsGroupsRepository.GetAchievementsGroupsAsync();

            return groups?.Select(g => g.Ahievements.OrderBy(a => a.NumberInGroup));
        }
    }
}
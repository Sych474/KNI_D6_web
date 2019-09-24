using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KNI_D6_web.Model.Achievements;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database.Repositories;
using Microsoft.AspNetCore.Http;
using System;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class AchievementGroupsController : Controller
    {
        private readonly IAchievementsGroupsRepository achievementsGroupsRepository;

        public AchievementGroupsController(IAchievementsGroupsRepository achievementsGroupsRepository)
        {
            this.achievementsGroupsRepository = achievementsGroupsRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await achievementsGroupsRepository.GetAchievementsGroupsAsync());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] AchievementsGroup achievementGroup)
        {
            if (!ModelState.IsValid)
                return View(achievementGroup);

            IActionResult result;
            try
            {
                await achievementsGroupsRepository.AddAchievementsGroupAsync(achievementGroup);
                result = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            
            var achievementGroup = await achievementsGroupsRepository.FindAchievementsGroupByIdAsync(id.Value);
            if (achievementGroup == null)
                return NotFound();

            return View(achievementGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] AchievementsGroup achievementGroup)
        {
            if (id != achievementGroup.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(achievementGroup);

            IActionResult result;
            try
            {
                await achievementsGroupsRepository.AddAchievementsGroupAsync(achievementGroup);
                result = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var achievementGroup = await achievementsGroupsRepository.FindAchievementsGroupByIdAsync(id.Value);
            if (achievementGroup == null)
                return NotFound();

            return View(achievementGroup);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            IActionResult result;
            try
            {
                await achievementsGroupsRepository.RemoveAchievementsGroupByIdAsync(id);
                result = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }
    }
}

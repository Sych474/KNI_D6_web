using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database.Repositories;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ParametersController : Controller
    {
        private readonly IAchievementsManager achievementsManager;
        private readonly IParameterValuesRepository parameterValuesRepository;
        private readonly IParametersRepository parametersRepository;
        private readonly IAchievementsRepository achievementsRepository;
        private readonly UserManager<User> userManager;

        public ParametersController(IAchievementsManager achievementsManager, IParameterValuesRepository parameterValuesRepository, IParametersRepository parametersRepository, IAchievementsRepository achievementsRepository, UserManager<User> userManager)
        {
            this.achievementsManager = achievementsManager;
            this.parameterValuesRepository = parameterValuesRepository;
            this.parametersRepository = parametersRepository;
            this.achievementsRepository = achievementsRepository;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await parametersRepository.GetParametersAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Parameter parameter)
        {
            IActionResult result = View(parameter);
            if (ModelState.IsValid)
            {
                try
                {
                    int parameterId = await parametersRepository.AddParameterAsync(parameter);

                    await parameterValuesRepository.AddParameterValuesForParameterAsync(parameterId);

                    result = RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    result = StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }
            return result;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult result = NotFound();
            if (id != null)
            {
                var parameter = await parametersRepository.FindParameterByIdAsync(id.Value);
                if (parameter != null)
                    result = View(parameter);
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Parameter parameter)
        {
            if (id != parameter.Id)
                return NotFound(id);

            if (!ModelState.IsValid)
                return View(parameter);

            IActionResult result;

            try
            {
                await parametersRepository.UpdateParameterAsync(parameter);
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

            var parameter = await parametersRepository.FindParameterByIdAsync(id.Value);
            if (parameter == null)
                return NotFound();

            return View(parameter);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            IActionResult result;
            try
            {
                await parametersRepository.RemoveParameterByIdAsync(id);
                result = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        public async Task<IActionResult> IncrementParameterValue(int parameterId, string userId)
        {
            if (await parametersRepository.FindParameterByIdAsync(parameterId) == null)
                return NotFound($"Parameter with Id {parameterId} not found");

            if (await userManager.FindByIdAsync(userId) == null)
                return NotFound($"User with Id {userId} not found");

            IActionResult result = NotFound(parameterId);
            try
            {
                await parameterValuesRepository.IncrementParamenterValueForUserAsync(parameterId, userId);

                var achievements = await achievementsRepository.GetAchievementsAsync();
                await achievementsManager.CheckAndUpdateСalculatedAchievementsForUser(userId, achievements.Select(a => a.Id));
                result = Redirect($"/Users/{userId}");
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        public async Task<IActionResult> DecrementParameterValue(int parameterId, string userId)
        {
            if (await parametersRepository.FindParameterByIdAsync(parameterId) == null)
                return NotFound($"Parameter with Id {parameterId} not found");

            if (await userManager.FindByIdAsync(userId) == null)
                return NotFound($"User with Id {userId} not found");

            IActionResult result = NotFound(parameterId);
            try
            {
                await parameterValuesRepository.DecrementParamenterValueForUserAsync(parameterId, userId);

                var achievements = await achievementsRepository.GetAchievementsAsync();
                await achievementsManager.CheckAndUpdateСalculatedAchievementsForUser(userId, achievements.Select(a => a.Id));
                result = Redirect($"/Users/{userId}");
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            return result;
        }
    }
}

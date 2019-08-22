using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KNI_D6_web.Model.Database;
using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ParametersController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAchievementsManager achievementsManager;

        public ParametersController(ApplicationDbContext dbContext, IAchievementsManager achievementsManager)
        {
            this.dbContext = dbContext;
            this.achievementsManager = achievementsManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await dbContext.Parameters.ToListAsync());
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
                dbContext.Add(parameter);
                
                await dbContext.SaveChangesAsync();
    
                foreach (var user in dbContext.Users)
                {
                        dbContext.ParameterValues.Add(new ParameterValue() { ParameterId = parameter.Id, UserId = user.Id});
                }
                await dbContext.SaveChangesAsync();
                result = RedirectToAction(nameof(Index));
            }
            return result;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult result = NotFound();
            if (id != null)
            {
                var parameter = await dbContext.Parameters.FindAsync(id);
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
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(parameter);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParameterExists(parameter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(parameter);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parameter = await dbContext.Parameters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parameter == null)
            {
                return NotFound();
            }

            return View(parameter);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parameter = await dbContext.Parameters.FindAsync(id);
            dbContext.Parameters.Remove(parameter);
            dbContext.ParameterValues.RemoveRange(dbContext.ParameterValues.Where(pv => pv.ParameterId == id));
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        public async Task<IActionResult> IncrementParameterValue(int parameterId, string userId)
        {
            IActionResult result = NotFound(parameterId);

            var parameterValue = await dbContext.ParameterValues.FirstOrDefaultAsync(pv => pv.ParameterId == parameterId && pv.UserId == userId);
            if (parameterValue != null)
            {
                parameterValue.Value++;
                dbContext.ParameterValues.Update(parameterValue);

                await dbContext.SaveChangesAsync();
                await achievementsManager.CheckAndUpdateСalculatedAchievementsForUser(userId, dbContext.Achievements.Select(a => a.Id));
                result = Redirect($"/Users/{userId}");
            }
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        public async Task<IActionResult> DecrementParameterValue(int parameterId, string userId)
        {
            IActionResult result = NotFound(parameterId);

            var parameterValue = await dbContext.ParameterValues.FirstOrDefaultAsync(pv => pv.ParameterId == parameterId && pv.UserId == userId);
            if (parameterValue != null)
            {
                parameterValue.Value--;
                dbContext.ParameterValues.Update(parameterValue);

                await dbContext.SaveChangesAsync();
                await achievementsManager.CheckAndUpdateСalculatedAchievementsForUser(userId, dbContext.Achievements.Select(a => a.Id));
                result = Redirect($"/Users/{userId}");
            }
            return result;
        }

        private bool ParameterExists(int id)
        {
            return dbContext.Parameters.Any(e => e.Id == id);
        }
    }
}

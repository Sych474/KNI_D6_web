using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.Model;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class AchievementGroupsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AchievementGroupsController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        // GET: AchievementGroups
        public async Task<IActionResult> Index()
        {
            return View(await dbContext.AchievementsGroups.ToListAsync());
        }


        // GET: AchievementGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] AchievementsGroup achievementGroup)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(achievementGroup);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(achievementGroup);
        }

        // GET: AchievementGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievementGroup = await dbContext.AchievementsGroups.FindAsync(id);
            if (achievementGroup == null)
            {
                return NotFound();
            }
            return View(achievementGroup);
        }

        // POST: AchievementGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] AchievementsGroup achievementGroup)
        {
            if (id != achievementGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(achievementGroup);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AchievementGroupExists(achievementGroup.Id))
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
            return View(achievementGroup);
        }

        // GET: AchievementGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievementGroup = await dbContext.AchievementsGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (achievementGroup == null)
            {
                return NotFound();
            }

            return View(achievementGroup);
        }

        // POST: AchievementGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var achievementGroup = await dbContext.AchievementsGroups.FindAsync(id);
            dbContext.AchievementsGroups.Remove(achievementGroup);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AchievementGroupExists(int id)
        {
            return dbContext.AchievementsGroups.Any(e => e.Id == id);
        }
    }
}

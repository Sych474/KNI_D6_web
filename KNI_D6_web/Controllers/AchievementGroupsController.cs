using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.Model;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.AdminRole)]
    public class AchievementGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AchievementGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AchievementGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.AchievementGroups.ToListAsync());
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
                _context.Add(achievementGroup);
                await _context.SaveChangesAsync();
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

            var achievementGroup = await _context.AchievementGroups.FindAsync(id);
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
                    _context.Update(achievementGroup);
                    await _context.SaveChangesAsync();
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

            var achievementGroup = await _context.AchievementGroups
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
            var achievementGroup = await _context.AchievementGroups.FindAsync(id);
            _context.AchievementGroups.Remove(achievementGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AchievementGroupExists(int id)
        {
            return _context.AchievementGroups.Any(e => e.Id == id);
        }
    }
}

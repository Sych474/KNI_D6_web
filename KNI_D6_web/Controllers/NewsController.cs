using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using KNI_D6_web.ViewModels.News;
using Microsoft.AspNetCore.Authorization;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.AdminRole)]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public NewsController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var viewModel = new NewsViewModel()
            {
                NewsPosts = await dbContext.NewsPosts
                    .Include(u => u.Author)
                    .OrderByDescending(x => x.PublicationDate).ToListAsync(),
                IsAdmin = this.User.IsInRole(UserRoles.AdminRole)
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PublicationDate,Article,Text,AuthorId")] NewsPost newsPost)
        {
            if (ModelState.IsValid)
            {
                newsPost.PublicationDate = DateTime.Now;
                newsPost.AuthorId = (await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name))?.Id;

                dbContext.Add(newsPost);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(newsPost);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsPost = await dbContext.NewsPosts.FindAsync(id);
            if (newsPost == null)
            {
                return NotFound();
            }

            ViewData["AuthorId"] = new SelectList(dbContext.Users, "Id", "UserName");
            return View(newsPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PublicationDate,Article,Text,AuthorId")] NewsPost newsPost)
        {
            if (id != newsPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(newsPost);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsPostExists(newsPost.Id))
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

            ViewData["AuthorId"] = new SelectList(dbContext.Users, "Id", "UserName", newsPost.AuthorId);
            return View(newsPost);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsPost = await dbContext.NewsPosts
                .Include(n => n.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsPost == null)
            {
                return NotFound();
            }

            return View(newsPost);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newsPost = await dbContext.NewsPosts.FindAsync(id);
            dbContext.NewsPosts.Remove(newsPost);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsPostExists(int id)
        {
            return dbContext.NewsPosts.Any(e => e.Id == id);
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.AdminAndModerator)]
    public class SemestersController : Controller
    {
        private readonly ISemestersRepository semestersRepository;

        public SemestersController(ISemestersRepository repository)
        {
            semestersRepository = repository;
        }

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            return View(await semestersRepository.GetSemestersAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Semester semester)
        {
            if (ModelState.IsValid)
            {
                await semestersRepository.AddSemesterAsync(semester);
                return RedirectToAction(nameof(Index));
            }
            return View(semester);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await semestersRepository.FindSemesterByIdAsync(id.Value);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsCurrent")] Semester semester)
        {
            if (id != semester.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(semester);

            if (await semestersRepository.UpdateSemesterAsync(semester))
                return RedirectToAction(nameof(Index));
            else
                return BadRequest();            
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var semester = await semestersRepository.FindSemesterByIdAsync(id.Value);

            if (semester == null)
                return NotFound();
            else
                return View(semester);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await semestersRepository.RemoveSemesterByIdAsync(id))
                return RedirectToAction(nameof(Index));
            else
                return BadRequest();
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> SetAsCurrent(int id)
        {
            if (await semestersRepository.SetSemesterAsCurrentByIdAsync(id))
                return RedirectToAction(nameof(Index));
            else
                return BadRequest();
        }
    }
}

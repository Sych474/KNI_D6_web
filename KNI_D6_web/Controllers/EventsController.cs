using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.ViewModels.Events;
using Microsoft.AspNetCore.Mvc.Rendering;
using KNI_D6_web.Model.Database.Repositories;
using System.Collections.Generic;
using System;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.AdminAndModerator)]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISemestersRepository semestersRepository;

        public EventsController(ApplicationDbContext context, ISemestersRepository semestersRepository)
        {
            this.dbContext = context;
            this.semestersRepository = semestersRepository;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var semester = await semestersRepository.FindCurrentSemesterAsync();
            var events = semester != null ?
                await dbContext.Events.Where(e => e.SemesterId == semester.Id || e.SemesterId == null).OrderBy(x => x.Date).ToListAsync() :
                await dbContext.Events.OrderBy(x => x.Date).ToListAsync();

            var futureEvents = events.Where(e => e.Date > DateTime.Now.Date);

            var pastEvents = events.Except(futureEvents);

            var viewModel = new EventsViewModel()
            {
                PastEvents = pastEvents,
                FutureEvents = futureEvents
            };

            return View(viewModel);
        }


        [HttpGet, Route("[controller]/All")]
        public async Task<IActionResult> All()
        {
            var events = await dbContext.Events.OrderByDescending(x => x.Date).ToListAsync();
            var futureEvents = events.Where(e => e.Date > DateTime.Now.Date);
            var pastEvents = events.Except(futureEvents);

            var viewModel = new EventsViewModel()
            {
                PastEvents = pastEvents,
                FutureEvents = futureEvents
            };

            return View("Index", viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await dbContext.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        public IActionResult Create()
        {
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(null);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Date,SemesterId,IsSpecial")] Event @event)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(@event);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(@event.SemesterId);
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
         
            var entity = await dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return NotFound();

            ViewData["SemestersSelectList"] = CreateSemestersSelectList(null);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Date,SemesterId,IsSpecial")] Event @event)
        {
            if (id != @event.Id)
                return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(@event);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            ViewData["SemestersSelectList"] = CreateSemestersSelectList(@event.SemesterId);
            return View(@event);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await dbContext.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await dbContext.Events.FindAsync(id);
            dbContext.Events.Remove(@event);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, Route("[controller]/EventVisitors/{id}")]
        public async Task<IActionResult> EventVisitors(int id)
        {
            IActionResult result = NotFound();
            if (await dbContext.Events.AnyAsync(e => e.Id == id))
            {
                var viewModel = new EventVisitorsViewModel()
                {
                    EventId = id,
                    EventVisitors = dbContext.Users.Include(u => u.UserEvents).Select(u => new EventVisitorViewModel()
                    {
                        Login = u.UserName,
                        UserId = u.Id,
                        IsVisited = u.UserEvents.Any(ue => ue.EventId == id)
                    })
                };
                result = View(viewModel);
            }
            return result;
        }
        
        [Route("[controller]/RemoveUserEvent")]
        public async Task<IActionResult> RemoveUserEvent(string userId, int eventId)
        {
            IActionResult result = NotFound();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userEvent = await dbContext.UserEvents.FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);
                if (userEvent != null)
                {
                    dbContext.UserEvents.Remove(userEvent);
                    await dbContext.SaveChangesAsync();
                    result = RedirectToAction(nameof(EventVisitors), new { id = eventId });
                }
            }
            return result;
        }

        [Route("[controller]/AddUserEvent")]
        public async Task<IActionResult> AddUserEvent(string userId, int eventId)
        {
            IActionResult result = NotFound();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userEvent = await dbContext.UserEvents.FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);
                if (userEvent == null)
                {
                    dbContext.UserEvents.Add(new UserEvent() { UserId = userId, EventId = eventId});
                    await dbContext.SaveChangesAsync();
                    result = RedirectToAction(nameof(EventVisitors), new { id = eventId });
                }
            }
            return result;
        }

        private bool EventExists(int id)
        {
            return dbContext.Events.Any(e => e.Id == id);
        }

        private SelectList CreateSemestersSelectList(int? currentId)
        {
            var tmpList = dbContext.Semesters.Select(s => new CustomSelectListItem() { Name = s.Name, Id = s.Id }).ToList();
            tmpList.Insert(0, new CustomSelectListItem() { Name = "Без семестра", Id = null });
            return new SelectList(tmpList, "Id", "Name", currentId);
        }

    }
}

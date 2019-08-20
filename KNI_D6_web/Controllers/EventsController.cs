﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.ViewModels.Events;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.AdminAndModerator)]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EventsController(ApplicationDbContext context)
        {
            this.dbContext = context;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var viewModel = new EventsViewModel()
            {
                Events = await dbContext.Events.OrderBy(x => x.Date).ToListAsync(),
            };
            return View(viewModel);
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Date")] Event @event)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(@event);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await dbContext.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

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
    }
}

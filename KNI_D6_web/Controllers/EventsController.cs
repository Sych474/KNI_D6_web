using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KNI_D6_web.Model;
using Microsoft.AspNetCore.Authorization;
using KNI_D6_web.ViewModels.Events;
using Microsoft.AspNetCore.Mvc.Rendering;
using KNI_D6_web.Model.Database.Repositories;
using System;
using Microsoft.AspNetCore.Http;

namespace KNI_D6_web.Controllers
{
    [Authorize(Roles = UserRoles.AdminAndModerator)]
    public class EventsController : Controller
    {
        private readonly ISemestersRepository semestersRepository;
        private readonly IEventsRepository eventsRepository;
        private readonly IUserEventsRepository userEventsRepository;
        private readonly IUsersRepository usersRepository;

        public EventsController(ISemestersRepository semestersRepository, IEventsRepository eventsRepository, IUserEventsRepository userEventsRepository, IUsersRepository usersRepository)
        {
            this.semestersRepository = semestersRepository;
            this.eventsRepository = eventsRepository;
            this.userEventsRepository = userEventsRepository;
            this.usersRepository = usersRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var semester = await semestersRepository.FindCurrentSemesterAsync();
            var events = semester != null ?
                (await eventsRepository.GetEventsAsync(semester.Id)).OrderBy(x => x.Date) :
                (await eventsRepository.GetEventsAsync()).OrderBy(x => x.Date);

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
            var events = (await eventsRepository.GetEventsAsync()).OrderByDescending(x => x.Date);
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
                return NotFound();
            var entity = await eventsRepository.FindEventByIdAsync(id.Value);

            if (entity == null)
                return NotFound();

            return View(entity);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["SemestersSelectList"] = await CreateSemestersSelectList(null);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Date,SemesterId,IsSpecial")] Event @event)
        {
            if (!ModelState.IsValid)
            {
                ViewData["SemestersSelectList"] = await CreateSemestersSelectList(@event.SemesterId);
                return View(@event);
            }

            IActionResult result;
            try
            {
                await eventsRepository.AddEventAsync(@event);
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

            var entity = await eventsRepository.FindEventByIdAsync(id.Value);
            if (entity == null)
                return NotFound();

            ViewData["SemestersSelectList"] = await CreateSemestersSelectList(null);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Date,SemesterId,IsSpecial")] Event @event)
        {
            if (!ModelState.IsValid)
            {
                ViewData["SemestersSelectList"] = await CreateSemestersSelectList(@event.SemesterId);
                return View(@event);
            }

            if (id != @event.Id)
                return BadRequest();

            IActionResult result;
            try
            {
                await eventsRepository.UpdateEventAsync(@event);
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

            var entity = await eventsRepository.FindEventByIdAsync(id.Value);
            if (entity == null)
                return NotFound();

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            IActionResult result;
            try
            {
                await eventsRepository.RemoveEventByIdAsync(id);
                result = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        [HttpGet, Route("[controller]/EventVisitors/{id}")]
        public async Task<IActionResult> EventVisitors(int id)
        {
            var entity = await eventsRepository.FindEventByIdAsync(id);
            if (entity == null)
                return NotFound();

            var visitors = (await usersRepository.GetUsersWithLinksAsync()).Select(u => new EventVisitorViewModel()
            {
                Login = u.UserName,
                UserId = u.Id,
                IsVisited = u.UserEvents.Any(ue => ue.EventId == id)
            }).ToList();
            visitors.Sort((x, y) => string.Compare(x.Login, y.Login));

            var viewModel = new EventVisitorsViewModel()
            {
                EventId = id,
                EventVisitors = visitors
            };
            return View(viewModel);
        }
        
        [Route("[controller]/RemoveUserEvent")]
        public async Task<IActionResult> RemoveUserEvent(string userId, int eventId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            IActionResult result;
            try
            {
                await userEventsRepository.RemoveUserEventAsync(userId, eventId);
                result = RedirectToAction(nameof(EventVisitors), new { id = eventId });
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        [Route("[controller]/AddUserEvent")]
        public async Task<IActionResult> AddUserEvent(string userId, int eventId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var userEvent = await userEventsRepository.FindUserEventAsync(userId, eventId);
            if (userEvent != null)
                return BadRequest();

            IActionResult result;
            try
            {
                await userEventsRepository.AddUserEventAsync(new UserEvent() { UserId = userId, EventId = eventId });
                result = RedirectToAction(nameof(EventVisitors), new { id = eventId });
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        private async Task<SelectList> CreateSemestersSelectList(int? currentId)
        {
            var tmpList = (await semestersRepository.GetSemestersAsync()).Select(s => new CustomSelectListItem() { Name = s.Name, Id = s.Id }).ToList();
            tmpList.Insert(0, new CustomSelectListItem() { Name = "Без семестра", Id = null });
            return new SelectList(tmpList, "Id", "Name", currentId);
        }
    }
}

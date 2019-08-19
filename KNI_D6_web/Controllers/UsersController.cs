using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using KNI_D6_web.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new UsersViewModel()
            {
                Users = await dbContext.Users
                    .Include(u => u.UserEvents).ThenInclude(ue => ue.Event)
                    .Include(u => u.UserAchievements)
                    .OrderByDescending(u => u.UserAchievements.Count())
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [Route("{id}")]
        public async Task<IActionResult> UserDetails(string id)
        {
            IActionResult result = BadRequest(id);
            var user = await dbContext.Users
                .Include(x => x.ParameterValues).ThenInclude(pv => pv.Parameter)
                .Include(x => x.UserAchievements).ThenInclude(pv => pv.Achievement)
                .Include(x => x.UserEvents).ThenInclude(pv => pv.Event)
                .Where(u => u.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                result = View(new UserDetailsViewModel()
                {
                    IsAuthorizedUser = User.Identity.IsAuthenticated && (User.Identity.Name == user.UserName),
                    User = user,
                    EventsViewModels = CreateEventViewModelsForUser(await dbContext.Events.ToListAsync(), user).OrderBy(x => x.EventDate)
                }) ;
            }

            return result;
        }

        [Route("UserDetails/{login}")]
        public async Task<IActionResult> UserDetailsByLogin(string login)
        {
            IActionResult result = BadRequest(login);

            var _user = await dbContext.Users.Where(u => u.UserName == login).FirstOrDefaultAsync();
            if (_user != null)
                result = RedirectToAction("UserDetails", "Users", new { id = _user.Id });

            return result;
        }

        [Route("Admins")]
        public async Task<IActionResult> Admins()
        {
            var admins = await dbContext.Users.Where(u => u.Position == UserPosition.Admin || u.Position == UserPosition.Secretary || u.Position == UserPosition.Chairman).ToListAsync();
            //For test
            var sych = await dbContext.Users.Where(u => u.UserName == "sych").FirstOrDefaultAsync();
            admins = new List<User>()
            {
                new User()
                {
                    UserName = "Свят",
                    Id = sych.Id,
                    Position = UserPosition.Chairman
                },
                new User()
                {
                    UserName = "Юра",
                    Id = sych.Id,
                    Position = UserPosition.Secretary
                },
                new User()
                {
                    UserName = "Саша Хляпов",
                    Id = sych.Id,
                    Position = UserPosition.Admin
                },
                new User()
                {
                    UserName = "Саша Васильев",
                    Id = sych.Id,
                    Position = UserPosition.Admin
                },
                new User()
                {
                    UserName = "Ратибор",
                    Id = sych.Id,
                    Position = UserPosition.Admin
                },
                new User()
                {
                    UserName = "Катя",
                    Id = sych.Id,
                    Position = UserPosition.Admin
                },
                new User()
                {
                    UserName = "Вит?",
                    Id = sych.Id,
                    Position = UserPosition.Admin
                },
            };
            return View(new AdminsViewModel() { Admins = admins });
        }

        private IEnumerable<UserDetailsEventViewModel> CreateEventViewModelsForUser(IEnumerable<Event> events, User user)
        {
            var result = new List<UserDetailsEventViewModel>();

            var visitedEventIds = user.UserEvents.Select(x => x.EventId);

            foreach (var item in events)
            {
                result.Add(new UserDetailsEventViewModel(item, visitedEventIds.Contains(item.Id)));
            }

            return result;
        }
    }
}
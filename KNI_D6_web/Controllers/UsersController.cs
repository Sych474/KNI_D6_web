using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;
using KNI_D6_web.Model.Database;
using KNI_D6_web.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly IAchievementsManager achievementsManager;

        public UsersController(ApplicationDbContext dbContext, UserManager<User> userManager, IAchievementsManager achievementsManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.achievementsManager = achievementsManager;
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

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            IActionResult result = NotFound();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    var userMaximumRole = UserRoles.GetMaximumRole(await userManager.GetRolesAsync(user));
                    var viewModel = new EditUserViewModel()
                    {
                        Login = user.UserName,
                        UserId = user.Id,
                        UserPosition = user.Position,
                        UserRole = userMaximumRole
                    };
                    result = View(viewModel);
                }
            }
            return result;
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,UserRole,UserPosition,Login")] EditUserViewModel viewModel)
        {
            IActionResult result = NotFound();
            if (ModelState.IsValid)
            {
                if (id == viewModel.UserId)
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                    if (user != null)
                    {
                        await UpdateUser(user, viewModel);

                        dbContext.Users.Update(user);

                        await dbContext.SaveChangesAsync();

                        result = RedirectToAction(nameof(Index));
                    }
                }
            }
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        [Route("ManageAchievements/{userId}")]
        public async Task<IActionResult> ManageAchievements(string userId)
        {
            IActionResult result = NotFound(userId);
            var user = await dbContext.Users.Include(u=> u.UserAchievements).FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var viewModel = new ManageAchievementsViewModel()
                {
                    Login = user.UserName,
                    UserId = userId,
                    Achievements = CreateAchievementsViewModel(user)
                };
                result = View(viewModel);
            }
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        [Route("{userId}/RemoveAchievement/{achievementId}")]
        public async Task<IActionResult> RemoveAchievement(string userId, int achievementId)
        {
            IActionResult result = NotFound();
            if (userId != null)
            {
                if (await achievementsManager.RemoveUserAchievement(achievementId, userId))
                    result = RedirectToAction(nameof(ManageAchievements), new { userId = userId });
            }
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        [Route("{userId}/AddAchievement/{achievementId}")]
        public async Task<IActionResult> AddAchievement(string userId, int achievementId)
        {
            IActionResult result = NotFound();
            if (userId != null)
            {
                if (await achievementsManager.AddAchievementToUser(achievementId, userId))
                    result = RedirectToAction(nameof(ManageAchievements), new { userId = userId });
            }
            return result;
        }

        private async Task UpdateUser(User user, EditUserViewModel viewModel)
        {
            var newRoles = UserRoles.GetAllRolesByMaximumRole(viewModel.UserRole);

            await userManager.RemoveFromRolesAsync(user, UserRoles.Roles);
            await userManager.AddToRolesAsync(user, newRoles);

            user.Position = viewModel.UserPosition;
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

        private IEnumerable<AchievementViewModel> CreateAchievementsViewModel(User user)
        {
            return dbContext.Achievements.Select(a => new AchievementViewModel()
            {
                Id = a.Id,
                Name = a.Name,
                IsReceived = user.UserAchievements.Any(ua => ua.AchievementId == a.Id)
            });
        }
    }
}
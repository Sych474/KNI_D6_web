using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database.Repositories;
using KNI_D6_web.Model.Parameters;
using KNI_D6_web.ViewModels.Components.AchievementsProgress;
using KNI_D6_web.ViewModels.Users;
using KNI_D6_web.ViewModels.Users.UserDetailsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IUsersRepository usersRepository;
        private readonly IAchievementsRepository achievementsRepository;
        private readonly IUserAchievementsRepository userAchievementsRepository;
        private readonly IEventsRepository eventsRepository;

        public UsersController(UserManager<User> userManager, IUsersRepository usersRepository, IAchievementsRepository achievementsRepository, IUserAchievementsRepository userAchievementsRepository, IEventsRepository eventsRepository)
        {
            this.userManager = userManager;
            this.usersRepository = usersRepository;
            this.achievementsRepository = achievementsRepository;
            this.userAchievementsRepository = userAchievementsRepository;
            this.eventsRepository = eventsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new UsersViewModel()
            {
                Users = (await usersRepository.GetUsersWithLinksAsync())
                    .OrderByDescending(u => u.UserAchievements.Count())
            };

            return View(viewModel);
        }

        [Route("{id}")]
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await usersRepository.FindFullUserByIdAsync(id);
            if (user == null)
                return NotFound(id);

            var viewModel = new UserDetailsViewModel()
            {
                UserId = id,
                UserName = user.UserName,
                Position = user.Position,
                Parameters = CreateUserDetailsParameterViewModels(user.ParameterValues),
                Achievements = CreateUserDetailsAchievementViewModels(user.UserAchievements),
                Events = await CreateUserDetailsEventViewModels(user.UserEvents)
            };

            return View(viewModel);
        }

        [HttpGet("UserDetails/{login}")]
        public async Task<IActionResult> UserDetailsByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return BadRequest();

            var user = await userManager.FindByNameAsync(login);
            if (user == null)
                return NotFound(login);

            return RedirectToAction("UserDetails", "Users", new { id = user.Id });
        }

        [HttpGet("Admins")]
        public async Task<IActionResult> Admins()
        {
            var secretary = (await usersRepository.GetUsersByPositionAsync(UserPosition.Secretary)).FirstOrDefault();
            var chairman = (await usersRepository.GetUsersByPositionAsync(UserPosition.Chairman)).FirstOrDefault();
            var admins = await usersRepository.GetUsersByPositionAsync(UserPosition.Admin);

            var fullList = new List<User>(admins);
            if (secretary != null)
                fullList.Insert(0, secretary);
            if (chairman != null)
                fullList.Insert(0, chairman);

            return View(new AdminsViewModel() { Admins = fullList });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(id);

            var viewModel = new EditUserViewModel()
            {
                Login = user.UserName,
                UserId = user.Id,
                UserPosition = user.Position,
                UserRole = UserRoles.GetMaximumRole(await userManager.GetRolesAsync(user))
            };
            return View(viewModel);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,UserRole,UserPosition,Login")] EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            if (id != viewModel.UserId)
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(id);

            IActionResult result;
            try
            {
                await UpdateUser(user, viewModel);
                await usersRepository.UpdateUserAsync(user);
                result = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        [HttpGet("ManageAchievements/{userId}")]
        public async Task<IActionResult> ManageAchievements(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var user = await usersRepository.FindUserWithAchievementsByIdAsync(userId);
            if (user == null)
                return NotFound(userId);

            var viewModel = new ManageAchievementsViewModel()
            {
                Login = user.UserName,
                UserId = userId,
                Achievements = await CreateAchievementsViewModel(user)
            };
            return View(viewModel);
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        [HttpGet("{userId}/RemoveAchievement/{achievementId}")]
        public async Task<IActionResult> RemoveAchievement(string userId, int achievementId)
        {
            if (userId == null)
                return NotFound();

            IActionResult result;
            try
            {
                await userAchievementsRepository.RemoveUserAchievementAsync(achievementId, userId);
                result = RedirectToAction(nameof(ManageAchievements), new { userId });
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        [Authorize(Roles = UserRoles.AdminAndModerator)]
        [HttpGet("{userId}/AddAchievement/{achievementId}")]
        public async Task<IActionResult> AddAchievement(string userId, int achievementId)
        {
            if (userId == null)
                return NotFound();

            IActionResult result;
            try
            {
                await userAchievementsRepository.AddUserAchievementAsync(achievementId, userId);
                result = RedirectToAction(nameof(ManageAchievements), new { userId });
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return result;
        }

        [HttpGet("{id}/Delete")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(id);

            return View(new DeleteUserViewModel() { Id = user.Id, Login = user.UserName });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("{id}/Delete"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
                await userManager.DeleteAsync(user);  
            
            return RedirectToAction(nameof(Index));
        }

        private async Task UpdateUser(User user, EditUserViewModel viewModel)
        {
            var newRoles = UserRoles.GetAllRolesByMaximumRole(viewModel.UserRole);

            var currRoles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, currRoles);
            await userManager.AddToRolesAsync(user, newRoles);

            user.Position = viewModel.UserPosition;
        }

        private async Task<IEnumerable<UserDetailsEventViewModel>> CreateUserDetailsEventViewModels(IEnumerable<UserEvent> userEvents)
        {
            var events = await eventsRepository.GetEventsAsync();
            var result = new List<UserDetailsEventViewModel>(events.Count());

            var visitedEventIds = userEvents.Select(x => x.EventId);

            foreach (var item in events)
            {
                result.Add(new UserDetailsEventViewModel()
                {
                    Date = item.Date,
                    EventId = item.Id,
                    EventName = item.Name, 
                    State = GetEventVisitState(item, visitedEventIds)
                });
            }

            result.Sort((x, y) => DateTime.Compare(x.Date, y.Date));
            return result;
        }

        private async Task<IEnumerable<AchievementProgressViewModel>> CreateAchievementsViewModel(User user)
        {
            var achievements = await achievementsRepository.GetAchievementsAsync();
            return achievements.Select(a => new AchievementProgressViewModel()
            {
                AchievementId = a.Id,
                AchievementName = a.Name,
                AchievementType = a.AchievementType,
                IsReceived = user.UserAchievements.Any(ua => ua.AchievementId == a.Id)
            });
        }

        private IEnumerable<UserDetailsAchievementViewModel> CreateUserDetailsAchievementViewModels(IEnumerable<UserAchievement> userAchievements)
        {
            return userAchievements.Select(ua => new UserDetailsAchievementViewModel()
            {
                Name = ua.Achievement.Name,
                Description = ua.Achievement.Description
            }).OrderBy(vm => vm.Name);
        }

        private IEnumerable<UserDetailsParameterViewModel> CreateUserDetailsParameterViewModels(IEnumerable<ParameterValue> parameterValues)
        {
            return parameterValues.Select(pv => new UserDetailsParameterViewModel()
            {
                Id = pv.ParameterId,
                Value = pv.Value,
                Name = pv.Parameter.Name
            }).OrderBy(vm => vm.Name);
        }

        private EventVisitState GetEventVisitState(Event item, IEnumerable<int> visitedEventIds)
        {
            var result = EventVisitState.NotVisited;

            if (item.Date.Date >= DateTime.Now.Date)
                result = EventVisitState.NotHappendYet;
            if (visitedEventIds.Contains(item.Id))
                result = EventVisitState.Visited;

            return result;
        }
    }
}
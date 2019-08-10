﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using KNI_D6_web.ViewModels.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
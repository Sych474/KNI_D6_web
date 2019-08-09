using System;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<User> _signInManager;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        // GET: Users/UserDetails/5
        [Route("{id}")]
        public IActionResult UserDetails(string id)
        {
            IActionResult result = BadRequest(id);
            var _user = _dbContext.Users
                .Include(x => x.ParameterValues).ThenInclude(pv => pv.Parameter)
                .Include(x => x.UserAchievements).ThenInclude(pv => pv.Achievement)
                .Where(u => u.Id == id).FirstOrDefault();

            if (_user != null)
            {
                result = View(new UserDetailsViewModel()
                {
                    IsAuthorizedUser = User.Identity.IsAuthenticated && (User.Identity.Name == _user.UserName),
                    User = _user,
                });
            }

            return result;
        }
    }
}
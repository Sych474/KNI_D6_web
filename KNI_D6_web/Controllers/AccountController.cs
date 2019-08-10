using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using KNI_D6_web.Model.Parameters;
using KNI_D6_web.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KNI_D6_web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _dbContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            IActionResult result = View(model);
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Login,
                };

                var identityResult = await _userManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    var currentUser = await _userManager.FindByNameAsync(model.Login);
                    foreach (var parameter in _dbContext.Parameters)
                        _dbContext.ParameterValues.Add(new ParameterValue() { ParameterId = parameter.Id, UserId = currentUser.Id, Value = 0 });
                    await _dbContext.SaveChangesAsync();

                    result = RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            IActionResult result = View(model);
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))                
                        result = Redirect(model.ReturnUrl);
                    else
                        result = RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Неверный логин и (или) пароль");
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
using System.Threading.Tasks;
using KNI_D6_web.Model;
using KNI_D6_web.Model.Database;
using KNI_D6_web.Model.Parameters;
using KNI_D6_web.Services;
using KNI_D6_web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KNI_D6_web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IEmailService emailService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext, IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
            this.emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("[controller]/{login}/ChangePassword")]
        public IActionResult ChangePassword(string login)
        {
            IActionResult result = Forbid();
            if (User.Identity.IsAuthenticated)
                if (User.IsInRole(UserRoles.Admin) || User.Identity.Name == login)
                    result = View(new ChangePasswordViewModel() { Login = login });
            
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
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

                var identityResult = await userManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    var currentUser = await userManager.FindByNameAsync(model.Login);
                    foreach (var parameter in dbContext.Parameters)
                        dbContext.ParameterValues.Add(new ParameterValue() { ParameterId = parameter.Id, UserId = currentUser.Id, Value = 0 });
                    await dbContext.SaveChangesAsync();

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
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            IActionResult result = View(model);
            if (ModelState.IsValid)
            {
                var signInResult = await signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
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
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("[controller]/{login}/ChangePassword")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            IActionResult result = View(model);
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var identityResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (identityResult.Succeeded)
                    result = RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("", "Ошибка изменения пароля");
            }
            return result;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    await emailService.SendEmailAsync(model.Email, "Reset Password",
                        $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
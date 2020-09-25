using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OneSignalApp.Models;
using OneSignalApp.Services;
using OneSignalApp.ViewModels;

namespace OneSignalApp.Controllers
{
  public class AccountController : Controller
  {
    public readonly ISecurityService securityService;
    public readonly IUserRepository userRepository;

    public AccountController(IUserRepository userRepository, ISecurityService securityService)
    {
      this.userRepository = userRepository;
      this.securityService = securityService;
    }

    public IActionResult Register()
    {
      return View(new User());
    }

    [HttpPost]
    public IActionResult Register(User user)
    {
      userRepository.RegisterUser(user);

      return View(user);
    }

    public IActionResult Login()
    {
      return View(new LoginInput());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginInput input)
    {
      var user = securityService.ValidateCredentials(input.Email, input.Password);

      if (user == null) return View(new LoginInput());

      await AuthHelper.SetAuthenticationCookie(HttpContext, user, false);
      return RedirectToAction("Index", "Apps");
    }

    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
      foreach (string cookiesKey in Request.Cookies.Keys)
      {
        Response.Cookies.Delete(cookiesKey);
      }
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return RedirectToAction("Login");
    }
  }
}
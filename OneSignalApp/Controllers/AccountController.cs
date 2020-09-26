using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OneSignalApp.Models;
using OneSignalApp.Services;
using OneSignalApp.ViewModels;

namespace OneSignalApp.Controllers
{
  public class AccountController : BaseController
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
      if (ModelState.IsValid)
      {
        var existingUser = userRepository.GetUserByEmail(user.Email);
        if (existingUser != null)
        {
          FlashError("This email is already in use.");
          return View(user);
        }

        userRepository.RegisterUser(user);
        FlashSuccess("User Registered Successfully.");
        return RedirectToAction("Login");
      }

      FlashError("Please fill all the fields");

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
      foreach (var cookiesKey in Request.Cookies.Keys) Response.Cookies.Delete(cookiesKey);
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return RedirectToAction("Login");
    }
  }
}
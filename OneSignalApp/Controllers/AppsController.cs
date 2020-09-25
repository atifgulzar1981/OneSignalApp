using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneSignalApp.Models;
using OneSignalApp.Services;

namespace OneSignalApp.Controllers
{
  [Authorize]
  public class AppsController : Controller
  {
    private readonly IUserService userService;

    public AppsController(IUserService userService)
    {
      this.userService = userService;
    }

    public IActionResult Index()
    {
      User loggedInUser = userService.GetLoggedInUser(HttpContext);

      //get apps from onesignal


      return View();
    }
  }
}
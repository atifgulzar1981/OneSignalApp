using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OneSignalApp.Models;
using OneSignalApp.Services;
using OneSignalApp.ViewModels;

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

    public async Task<IActionResult> Index()
    {
      var loggedInUser = userService.GetLoggedInUser(HttpContext);
      var apps = new List<App>();

      using (var httpClient = new HttpClient())
      {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {AppConstants.ONE_SIGNAL_AUTH_KEY}");
        using (var response = await httpClient.GetAsync(AppConstants.ONE_SIGNAL_APPS_API))
        {
          if (!response.IsSuccessStatusCode) return View(new AppsViewModel
          {
            Apps = apps,
            LoggedInUser = loggedInUser
          });

          var apiResponse = await response.Content.ReadAsStringAsync();
          if (!string.IsNullOrEmpty(apiResponse))
            apps = JsonConvert.DeserializeObject<List<App>>(apiResponse);
        }
      }

      return View(new AppsViewModel
      {
        Apps = apps,
        LoggedInUser = loggedInUser
      });
    }

    public IActionResult Create()
    {
      return View(new App());
    }

    public IActionResult Edit()
    {
      return View(new App());
    }
  }
}
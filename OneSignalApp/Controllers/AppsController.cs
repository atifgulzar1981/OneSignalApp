using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OneSignalApp.Models;
using OneSignalApp.Services;
using OneSignalApp.ViewModels;

namespace OneSignalApp.Controllers
{
  public class AppsController : BaseController
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
          if (!response.IsSuccessStatusCode)
            return View(new AppsViewModel
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

    [HttpPost]
    public async Task<IActionResult> Create(App app)
    {
      var newApp = new App
      {
        name = app.name,
        created_at = DateTime.UtcNow,
        apns_env = "production"
      };

      var httpWebRequest = (HttpWebRequest) WebRequest.Create(AppConstants.ONE_SIGNAL_APPS_API);
      httpWebRequest.Headers.Add("Authorization", $"Basic {AppConstants.ONE_SIGNAL_AUTH_KEY}");
      httpWebRequest.ContentType = "application/json";

      httpWebRequest.Method = "POST";

      await using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        var postDataJson = JsonConvert.SerializeObject(newApp);
        await streamWriter.WriteAsync(postDataJson);
      }

      var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();

      if (httpResponse.StatusCode == HttpStatusCode.OK)
      {
        using var streamReader = new StreamReader(httpResponse.GetResponseStream()!);
        var result = await streamReader.ReadToEndAsync();

        var savedApp = JsonConvert.DeserializeObject<App>(result);
        if (savedApp != null && !string.IsNullOrEmpty(savedApp.id))
        {
          FlashSuccess("App Saved Successfully.");
          return RedirectToAction("Index");
        }
      }

      FlashError("Error while saving app.");
      return View(newApp);
    }

    public async Task<IActionResult> Edit(string id)
    {
      App appToEdit = null;
      var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{AppConstants.ONE_SIGNAL_APPS_API}/{id}");
      httpWebRequest.Headers.Add("Authorization", $"Basic {AppConstants.ONE_SIGNAL_AUTH_KEY}");
      httpWebRequest.ContentType = "application/json";

      var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();

      if (httpResponse.StatusCode == HttpStatusCode.OK)
      {
        using var streamReader = new StreamReader(httpResponse.GetResponseStream()!);
        var result = await streamReader.ReadToEndAsync();

        appToEdit = JsonConvert.DeserializeObject<App>(result);
        if (appToEdit == null) return NotFound();
      }

      return View(appToEdit);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, App app)
    {
      var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{AppConstants.ONE_SIGNAL_APPS_API}/{id}");
      httpWebRequest.Headers.Add("Authorization", $"Basic {AppConstants.ONE_SIGNAL_AUTH_KEY}");
      httpWebRequest.ContentType = "application/json";

      httpWebRequest.Method = "PUT";

      await using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        var postDataJson = JsonConvert.SerializeObject(app);
        await streamWriter.WriteAsync(postDataJson);
      }

      var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();

      if (httpResponse.StatusCode == HttpStatusCode.OK)
      {
        using var streamReader = new StreamReader(httpResponse.GetResponseStream()!);
        var result = await streamReader.ReadToEndAsync();

        var updatedApp = JsonConvert.DeserializeObject<App>(result);
        if (updatedApp != null && updatedApp.name.ToLower().Equals(app.name.ToLower()))
        {
          FlashSuccess("App Updated Successfully.");
          return RedirectToAction("Index");
        }
      }

      FlashError("Error while updating app.");
      return View(app);
    }
  }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OneSignalApp.ActionFilters;
using OneSignalApp.Models;
using OneSignalApp.Services;
using OneSignalApp.ViewModels;

namespace OneSignalApp.Controllers
{
  [Authorize]
  public class AppsController : BaseController
  {
    private readonly IUserContext userContext;

    public AppsController(IUserContext userContext)
    {
      this.userContext = userContext;
    }

    public async Task<IActionResult> Index()
    {
      if (userContext.CurrentUser == null)
        return NotFound();

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
              LoggedInUser = userContext.CurrentUser
            });

          var apiResponse = await response.Content.ReadAsStringAsync();
          if (!string.IsNullOrEmpty(apiResponse))
            apps = JsonConvert.DeserializeObject<List<App>>(apiResponse);
        }
      }

      return View(new AppsViewModel
      {
        Apps = apps,
        LoggedInUser = userContext.CurrentUser
      });
    }

    [RequireAdmin]
    public IActionResult Create()
    {
      return View(new App());
    }

    [HttpPost]
    [RequireAdmin]
    public async Task<IActionResult> Create(App app)
    {
      if (!ModelState.IsValid)
      {
        FlashError("Please fill all the fields");
        return View(app);
      }

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

    [RequireAdmin]
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
    [RequireAdmin]
    public async Task<IActionResult> Edit(string id, App app)
    {
      if (!ModelState.IsValid)
      {
        FlashError("Please fill all the fields");
        return View(app);
      }

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
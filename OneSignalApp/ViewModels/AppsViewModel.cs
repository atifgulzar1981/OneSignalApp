using System.Collections.Generic;
using OneSignalApp.Models;

namespace OneSignalApp.ViewModels
{
  public class AppsViewModel
  {
    public List<App> Apps { get; set; }
    public User LoggedInUser { get; set; }
  }
}

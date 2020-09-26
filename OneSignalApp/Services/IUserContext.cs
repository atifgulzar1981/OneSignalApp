using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public interface IUserContext
  {
    bool IsLoggedIn { get; }
    User CurrentUser { get; }
  }
}
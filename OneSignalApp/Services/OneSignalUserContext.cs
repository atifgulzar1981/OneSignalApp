using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public class OneSignalUserContext : IUserContext
  {
    public bool IsLoggedIn { get; private set; }
    public User CurrentUser { get; private set; }

    public void SetCurrentUser(User user)
    {
      CurrentUser = user;
      IsLoggedIn = user != null;
    }
  }
}
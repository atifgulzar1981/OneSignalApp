using Microsoft.AspNetCore.Http;
using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public interface IUserService
  {
    User GetLoggedInUser(HttpContext context);
  }
}
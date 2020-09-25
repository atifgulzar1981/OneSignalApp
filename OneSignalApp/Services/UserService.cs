using System.Linq;
using Microsoft.AspNetCore.Http;
using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public class UserService : IUserService
  {
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
      this.userRepository = userRepository;
    }

    public User GetLoggedInUser(HttpContext context)
    {
      var userClaim = context.User.Claims?.FirstOrDefault();
      if (userClaim != null)
      {
        var loggedInUser = userRepository.GetUserByEmail(userClaim.Value);
        return loggedInUser;
      }

      return null;
    }
  }
}
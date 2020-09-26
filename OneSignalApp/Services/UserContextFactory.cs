using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OneSignalApp.DbContext;
using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public class UserContextFactory : IUserContextFactory
  {
    private readonly AppDbContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserContextFactory(
      AppDbContext dbContext,
      IHttpContextAccessor httpContextAccessor
    )
    {
      this.dbContext = dbContext;
      this.httpContextAccessor = httpContextAccessor;
    }

    public IUserContext GetUserContext()
    {
      var userContext = new OneSignalUserContext();
      var user = GetUser();

      if (user != null) userContext.SetCurrentUser(user);

      return userContext;
    }

    private User GetUser()
    {
      if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) return null;

      var emailAddress = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;

      if (string.IsNullOrWhiteSpace(emailAddress)) return null;

      var user = dbContext.Users.SingleOrDefault(x => x.Email == emailAddress);

      return user;
    }
  }
}
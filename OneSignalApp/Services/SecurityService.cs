using System.Linq;
using OneSignalApp.DbContext;
using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public class SecurityService : ISecurityService
  {
    public readonly AppDbContext dbContext;

    public SecurityService(AppDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public User ValidateCredentials(string userName, string password)
    {
      var user = dbContext.Users.FirstOrDefault(x => x.Email == userName);

      if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        return user;

      return null;
    }
  }
}
using System.Linq;
using OneSignalApp.DbContext;

namespace OneSignalApp.Models
{
  public class SqlUserRepository : IUserRepository
  {
    public readonly AppDbContext dbContext;

    public SqlUserRepository(AppDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public User GetUser(int id)
    {
      return dbContext.Users.Find(id);
    }

    public User GetUserByEmail(string email)
    {
      return dbContext.Users.FirstOrDefault(x => x.Email == email);
    }

    public User RegisterUser(User user)
    {
      user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
      dbContext.Users.Add(user);
      dbContext.SaveChanges();
      return user;
    }
  }
}
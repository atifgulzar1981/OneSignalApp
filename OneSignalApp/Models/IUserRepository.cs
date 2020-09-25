namespace OneSignalApp.Models
{
  public interface IUserRepository
  {
    User GetUser(int id);
    User GetUserByEmail(string email);
    User RegisterUser(User user);
  }
}
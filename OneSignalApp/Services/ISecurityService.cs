using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public interface ISecurityService
  {
    User ValidateCredentials(string userName, string password);
  }
}
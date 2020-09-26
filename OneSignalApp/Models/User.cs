using FluentValidation;

namespace OneSignalApp.Models
{
  public class User
  {
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; }
  }

  public class UserValidator : AbstractValidator<User>
  {
    public UserValidator()
    {
      RuleFor(x => x.FullName)
        .NotEmpty()
        .Length(2, 150);

      RuleFor(x => x.Email)
        .NotEmpty()
        .Length(1, 254)
        .EmailAddress();

      RuleFor(x => x.Password)
        .NotEmpty()
        .Length(8, 50);
    }
  }
}
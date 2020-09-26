using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using OneSignalApp.Models;
using OneSignalApp.Services;

namespace OneSignalApp.ActionFilters
{
  public class RequireAdminAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var userContext = context.HttpContext.RequestServices.GetService<IUserContext>();

      if (
        userContext == null
        || userContext.CurrentUser == null
        || userContext.CurrentUser.UserType != UserType.Admin
      )
        context.Result = new ContentResult
        {
          StatusCode = StatusCodes.Status404NotFound,
        };
    }
  }
}
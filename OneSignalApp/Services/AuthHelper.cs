using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OneSignalApp.Models;

namespace OneSignalApp.Services
{
  public static class AuthHelper
  {
    public static async Task SetAuthenticationCookie(HttpContext httpContext, User user, bool isPersistent)
    {
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email),
        new Claim("USERID", user.Id.ToString(), String.Empty),
      };

      var userIdentity = new ClaimsIdentity("OneSignalLogin");
      userIdentity.AddClaims(claims);

      var userPrincipal = new ClaimsPrincipal(userIdentity);

      var authenticationProperties = new AuthenticationProperties
      {
        IsPersistent = isPersistent
      };

      await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal,
        authenticationProperties);

      httpContext.User = userPrincipal;
    }
  }
}
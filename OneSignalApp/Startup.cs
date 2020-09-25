using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneSignalApp.DbContext;
using OneSignalApp.Migrations;
using OneSignalApp.Models;
using OneSignalApp.Services;

namespace OneSignalApp
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      //Run migrations
      var migrationHelper = new MigrationHelpers();
      migrationHelper.UpdateDatabase(configuration.GetConnectionString("Default"));
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();

      services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("Default")));

      services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
          options.Cookie.Name = "onesignal_app_cookie";
          options.Cookie.HttpOnly = true;
          options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
          options.AccessDeniedPath = new PathString("/forbidden");
          options.LoginPath = new PathString("/account/login");
          options.ExpireTimeSpan = TimeSpan.FromDays(15);
          options.SlidingExpiration = true;
        })
        ;

      services.AddTransient<ISecurityService, SecurityService>();
      services.AddTransient<IUserRepository, SqlUserRepository>();
      services.AddTransient<IUserService, UserService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          "default",
          "{controller=Apps}/{action=Index}/{id?}");
      });
    }
  }
}
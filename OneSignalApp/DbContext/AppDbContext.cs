using Microsoft.EntityFrameworkCore;
using OneSignalApp.Models;

namespace OneSignalApp.DbContext
{
  public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
  }
}
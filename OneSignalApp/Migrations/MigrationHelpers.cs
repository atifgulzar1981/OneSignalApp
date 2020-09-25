using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace OneSignalApp.Migrations
{
  public class MigrationHelpers
  {
    public void UpdateDatabase(string connectionString)
    {
      var service = new ServiceCollection()
          .AddFluentMigratorCore()
          .ConfigureRunner(options => options
            .AddSqlServer2016()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(MigrationHelpers).Assembly)
            .For.All()
          )
          .AddLogging(options => options.AddFluentMigratorConsole())
          .BuildServiceProvider(false)
        ;

      using var scope = service.CreateScope();
      var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
      runner.MigrateUp();
    }
  }
}
using Autofac;
using OneSignalApp.Services;

namespace OneSignalApp
{
  public class AutofacModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      base.Load(builder);

      builder.RegisterAssemblyTypes( typeof(Startup).Assembly)
        .PublicOnly()
        .AsImplementedInterfaces();

      builder.Register(x => x.Resolve<IUserContextFactory>().GetUserContext())
        .As<IUserContext>()
        .InstancePerLifetimeScope();
    }
  }
}
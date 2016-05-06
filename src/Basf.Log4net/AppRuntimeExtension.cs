using Basf.Logging;

namespace Basf.Log4net
{
    public static class AppRuntimeExtension
    {
        public static void UsingLog4Net(this AppRuntime objAppRuntime, string configFile = null)
        {
            AppRuntime.Register(f => f.RegisterType<ILogger, Log4NetLogger>()
            .UsingConstructor(typeof(string)).Lifetime(LifetimeStyle.Singleton)
            .WithParameter("configFile", configFile));
        }
    }
}

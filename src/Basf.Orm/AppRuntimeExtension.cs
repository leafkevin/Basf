using System;
using System.Data;

namespace Basf.Orm
{
    public static class AppRuntimeExtension
    {
        public static AppRuntime UsingOrmDapper<TOrmProvider>(this AppRuntime objAppRuntime, string connKey, string name = null)
            where TOrmProvider : class, IOrmProvider
        {
            if (String.IsNullOrEmpty(name))
            {
                AppRuntime.RegisterType<IOrmProvider, TOrmProvider>(LifetimeStyle.Singleton);
                AppRuntime.Register(f => f.RegisterType<IDbConnection, DapperConnection>().WithParameters(connKey).Lifetime(LifetimeStyle.Thread));
            }
            else
            {
                AppRuntime.RegisterType<IOrmProvider, TOrmProvider>(name, LifetimeStyle.Singleton);
                AppRuntime.Register(f => f.RegisterType<IDbConnection, DapperConnection>().WithParameters(connKey).Named<IDbConnection>(name).Lifetime(LifetimeStyle.Thread));
            }
            return AppRuntime.Instance;
        }
    }
}

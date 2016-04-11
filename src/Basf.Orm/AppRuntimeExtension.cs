using System;
using System.Data;

namespace Basf.Orm
{
    public static class AppRuntimeExtension
    {
        public static AppRuntime UsingOrmDapper<TConnection, TOrmProvider>(this AppRuntime objAppRuntime, string connKey, string name = null)
           where TConnection : class, IDbConnection
           where TOrmProvider : class, IOrmProvider
        {
            string connString = Utility.GetConnString(connKey);
            if (String.IsNullOrEmpty(name))
            {
                AppRuntime.Register(f => f.RegisterType<IDbConnection, TConnection>().WithParameters(connString).Lifetime(LifetimeStyle.Thread));
                AppRuntime.RegisterType<IOrmProvider, TOrmProvider>(LifetimeStyle.Singleton);
            }
            else
            {
                AppRuntime.Register(f => f.RegisterType<IDbConnection, TConnection>().WithParameters(connString).Named<IDbConnection>(name).Lifetime(LifetimeStyle.Thread));
                AppRuntime.RegisterType<IOrmProvider, TOrmProvider>(name, LifetimeStyle.Singleton);
            }
            return AppRuntime.Instance;
        }
    }
}

using Basf.Orm;
using System.Data;
using Basf.Domain.Repository;

namespace Basf.DapperRepository
{
    public static class AppRuntimeExtension
    {
        public static AppRuntime UsingRepository<TConnection, TOrmProvider>(this AppRuntime objAppRuntime, string connKey)
            where TConnection : class, IDbConnection
            where TOrmProvider : class, IOrmProvider
        {
            objAppRuntime.UsingOrmDapper<TConnection, TOrmProvider>(connKey);
            AppRuntime.RegisterGeneric(typeof(IRepository<>), typeof(Repository<>), LifetimeStyle.Thread);
            return AppRuntime.Instance;
        }
    }
}
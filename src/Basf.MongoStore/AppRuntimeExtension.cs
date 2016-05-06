using Basf.Domain.Storage;
using Basf.Logging;

namespace Basf.MongoStore
{
    public static class AppRuntimeExtension
    {
        public static void UsingMongoStore(this AppRuntime objAppRuntime)
        {
            //AppRuntime.RegisterType<ILogger, MongoLogger>(LifetimeStyle.Singleton);
            //AppRuntime.RegisterType<IEventStore, MongoEventStore>(LifetimeStyle.Singleton);
            //AppRuntime.RegisterType<ISnapshotStore, MongoSnapshotStore>(LifetimeStyle.Singleton);
        }
    }
}

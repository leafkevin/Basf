using Basf.Domain.Storage;

namespace Basf.MongoStore
{
    public static class AppRuntimeExtension
    {
        public static AppRuntime UsingMongoStore(this AppRuntime objAppRuntime)
        {
            AppRuntime.RegisterType<ICommandStore, MongoCommandStore>(LifetimeStyle.Singleton);
            AppRuntime.RegisterType<IEventStore, MongoEventStore>(LifetimeStyle.Singleton);
            AppRuntime.RegisterType<ISnapshotStore, MongoSnapshotStore>(LifetimeStyle.Singleton);
            return AppRuntime.Instance;
        }
    }
}

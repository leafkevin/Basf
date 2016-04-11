using Basf.Domain.Storage;

namespace Basf.CouchbaseStore
{
    public static class AppRuntimeExtension
    {
        public static void UsingCouchbaseStore(this AppRuntime objAppRuntime)
        {
            AppRuntime.RegisterType<ICommandStore, CouchbaseCommandStore>(LifetimeStyle.Singleton);
            AppRuntime.RegisterType<IEventStore, CouchbaseEventStore>(LifetimeStyle.Singleton);
            AppRuntime.RegisterType<ISnapshotStore, CouchbaseSnapshotStore>(LifetimeStyle.Singleton);
        }
    }
}

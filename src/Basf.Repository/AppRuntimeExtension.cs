namespace Basf.Repository
{
    public static class AppRuntimeExtension
    {
        public static AppRuntime UsingRepository<TOrmProvider>(this AppRuntime objAppRuntime, string connString, bool isDefault = true)
            where TOrmProvider : IOrmProvider, new()
        {
            OrmProviderFactory.RegisterProvider(connString, new TOrmProvider(), isDefault);
          
            AppRuntime.Register(f => f.RegisterGeneric(typeof(IRepository<>), typeof(Repository<>))
                .Lifetime(LifetimeStyle.Thread).WithParameter("connString", connString));
            AppRuntime.Register(f => f.RegisterType<IRepositoryContext, RepositoryContext>()
                .Lifetime(LifetimeStyle.Thread).WithParameter("connString", connString));
            return AppRuntime.Instance;
        }
    }
}
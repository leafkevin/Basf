using Basf.Domain.Command;
using Basf.Domain.Event;
using System;
using System.Linq;
using System.Reflection;

namespace Basf.Domain
{
    public static class AppRuntimeExtension
    {
        public static AppRuntime UsingDomain(this AppRuntime objAppRuntime, params Assembly[] assemblies)
        {
            //AppRuntime.RegisterType<ICommandContext, CommandContext>(LifetimeStyle.Singleton);
            //AppRuntime.RegisterType<IDomainContext, DomainContext>(LifetimeStyle.Singleton);
            RegisterHandlers(assemblies);
            return AppRuntime.Instance;
        }
        private static void RegisterHandlers(params Assembly[] assemblies)
        {
            if (assemblies != null)
            {
                RegisterCommandHandler(typeof(ICommandHandler<>), assemblies);
                RegisterEventHandler(typeof(IEventHandler<>), assemblies);
            }
        }
        private static void RegisterCommandHandler(Type handlerType, params Assembly[] assemblies)
        {
            var handlerTypes = assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => IsHandlerType(t, handlerType)));
            foreach (Type type in handlerTypes)
            {
                var ifaceTypes = type.GetInterfaces();
                foreach (Type ifaceType in ifaceTypes)
                {
                    AppRuntime.RegisterType(ifaceType, type, LifetimeStyle.Singleton);
                }
            }
        }
        private static void RegisterEventHandler(Type handlerType, params Assembly[] assemblies)
        {
            var aggRootTypes = assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => IsHandlerType(t, handlerType)));
            foreach (Type aggRootType in aggRootTypes)
            {
                Type eventType = aggRootType.GenericTypeArguments[0];
                AppRuntime.Resolve<IDomainContext>().AddHandler(aggRootType, eventType);
            }
        }
        private static bool IsHandlerType(Type type, Type HandlerType)
        {
            return type.IsClass && !type.IsAbstract && type.IsAssignableFrom(HandlerType);
        }
    }
}

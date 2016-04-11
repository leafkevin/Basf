using Basf.Domain.Command;
using Basf.Domain.Event;
using System;
using System.Linq;
using System.Reflection;

namespace Basf.Domain
{
    public static class AppRuntimeExtension
    {
        public static void RegisterHandlers(params Assembly[] assemblies)
        {
            if (assemblies != null)
            {
                //注册Command Handler
                Type handlerType = typeof(ICommandHandler<>);
                var types = assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => IsHandlerType(t, handlerType)));
                foreach (var type in types)
                {
                    AppRuntime.RegisterType(handlerType, type, LifetimeStyle.Singleton);
                }
                //注册Event Handler
                handlerType = typeof(IEventHandler<>);
                types = assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => IsHandlerType(t, handlerType)));
                foreach (var type in types)
                {
                    AppRuntime.RegisterType(handlerType, type, LifetimeStyle.Singleton);
                }
            }
        }
        private static bool IsHandlerType(Type type, Type handlerType)
        {
            return type != null && type.IsClass && !type.IsAbstract && type.IsAssignableFrom(handlerType);
        }
    }
}

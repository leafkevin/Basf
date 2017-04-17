using System;
using System.Collections.Generic;

namespace Basf
{
    public static class ObjectContainerExtensions
    {
        #region Register
        public static void RegisterType(this IObjectContainer objContainer, Type objServiceType, Type objComponentType)
        {
            objContainer.Register(f => f.RegisterType(objServiceType, objComponentType));
        }
        public static void RegisterType(this IObjectContainer objContainer, Type objServiceType, Type objComponentType, string strName)
        {
            objContainer.Register(f => f.RegisterType(objServiceType, objComponentType).Named(strName, objServiceType));
        }
        public static void RegisterType(this IObjectContainer objContainer, Type objServiceType, Type objComponentType, LifetimeStyle iLifetimeStyle)
        {
            objContainer.Register(f => f.RegisterType(objServiceType, objComponentType).Lifetime(iLifetimeStyle));
        }
        public static void RegisterType(this IObjectContainer objContainer, Type objServiceType, Type objComponentType, string strName, LifetimeStyle iLifetimeStyle)
        {
            objContainer.Register(f => f.RegisterType(objServiceType, objComponentType).Named(strName, objServiceType).Lifetime(iLifetimeStyle));
        }
        public static void RegisterType<TService>(this IObjectContainer objContainer) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>());
        }
        public static void RegisterType<TService>(this IObjectContainer objContainer, string strName) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>().Named<TService>(strName));
        }
        public static void RegisterType<TService>(this IObjectContainer objContainer, LifetimeStyle iLifetimeStyle) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>().Lifetime(iLifetimeStyle));
        }
        public static void RegisterType<TService>(this IObjectContainer objContainer, string strName, LifetimeStyle iLifetimeStyle) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>().Named<TService>(strName).Lifetime(iLifetimeStyle));
        }
        public static void RegisterType<TService, TComponent>(this IObjectContainer objContainer)
            where TService : class
            where TComponent : class, TService
        {
            objContainer.Register(f => f.RegisterType<TService, TComponent>());
        }
        public static void RegisterType<TService, TComponent>(this IObjectContainer objContainer, string strName)
            where TService : class
            where TComponent : class, TService
        {
            objContainer.Register(f => f.RegisterType<TService, TComponent>().Named<TService>(strName));
        }
        public static void RegisterType<TService, TComponent>(this IObjectContainer objContainer, LifetimeStyle iLifetimeStyle)
            where TService : class
            where TComponent : class, TService
        {
            objContainer.Register(f => f.RegisterType<TService, TComponent>().Lifetime(iLifetimeStyle));
        }
        public static void RegisterType<TService, TComponent>(this IObjectContainer objContainer, string strName, LifetimeStyle iLifetimeStyle)
            where TService : class
            where TComponent : class, TService
        {
            objContainer.Register(f => f.RegisterType<TService, TComponent>().Named<TService>(strName).Lifetime(iLifetimeStyle));
        }
        public static void RegisterInstance<TService>(this IObjectContainer objContainer, TService objInstance) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>(objInstance));
        }
        public static void RegisterInstance<TService>(this IObjectContainer objContainer, TService objInstance, string strName) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>(objInstance).Named<TService>(strName));
        }
        public static void RegisterInstance<TService>(this IObjectContainer objContainer, TService objInstance, LifetimeStyle iLifetimeStyle) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>(objInstance).Lifetime(iLifetimeStyle));
        }
        public static void RegisterInstance<TService>(this IObjectContainer objContainer, TService objInstance, string strName, LifetimeStyle iLifetimeStyle) where TService : class
        {
            objContainer.Register(f => f.RegisterType<TService>(objInstance).Named<TService>(strName).Lifetime(iLifetimeStyle));
        }
        public static void RegisterGeneric(this IObjectContainer objContainer, Type objService, Type objComponent)
        {
            objContainer.Register(f => f.RegisterGeneric(objService, objComponent).Lifetime(LifetimeStyle.Transient));
        }
        public static void RegisterGeneric(this IObjectContainer objContainer, Type objService, Type objComponent, LifetimeStyle iLifetimeStyle)
        {
            objContainer.Register(f => f.RegisterGeneric(objService, objComponent).Lifetime(iLifetimeStyle));
        }
        #endregion

        #region Resolve      
        public static TService Resolve<TService>(this IObjectContainer objContainer, params object[] objArgs) where TService : class
        {
            return (TService)objContainer.Resolve(typeof(TService), objArgs);
        }
        public static TService Resolve<TService>(this IObjectContainer objContainer, IDictionary<string, object> objArgs) where TService : class
        {
            return (TService)objContainer.Resolve(typeof(TService), objArgs);
        }
        public static bool HasRegister<TService>(this IObjectContainer objContainer)
        {
            return objContainer.HasRegister(typeof(TService));
        }
        public static bool HasRegister<TService>(this IObjectContainer objContainer, string strName)
        {
            return objContainer.HasRegister(typeof(TService), strName);
        }
        #endregion
    }
}

using Basf.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Basf
{
    public sealed class AppRuntime
    {
        #region 私有字段
        private static AppRuntime objInstance = new AppRuntime();
        private IObjectContainer objContainer = null;
        private List<ILogger> objLoggerList = null;
        #endregion

        #region 属性
        public static AppRuntime Instance { get { return AppRuntime.objInstance; } }
        #endregion

        #region 构造方法
        private AppRuntime()
        {
        }
        #endregion

        #region 配置方法
        public static void Configure(Action<AppRuntime> objBuilder)
        {
            objBuilder?.Invoke(AppRuntime.Instance);
        }
        public AppRuntime UsingContainer(IObjectContainer objContainer)
        {
            Utility.NotNull(objContainer, "objContainer");
            AppRuntime.Instance.objContainer = objContainer;
            return AppRuntime.Instance;
        }
        public AppRuntime UsingLogger<TLogger>() where TLogger : class, ILogger
        {
            Utility.Fail(objContainer == null, "请先调用AppRuntime.UsingContainer方法后再调用本方法！");
            AppRuntime.Instance.objContainer.RegisterType<ILogger, TLogger>(LifetimeStyle.Singleton);
            objLoggerList = AppRuntime.ResolveAll<ILogger>().ToList();
            return AppRuntime.Instance;
        }
        #endregion

        #region 日志方法
        public static void Info(object objMessage)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Info(objMessage)));
        }
        public static void Info(object objMessage, Exception objException)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Info(objMessage, objException)));
        }
        public static void InfoFormat(string strFormat, params object[] objArgs)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.InfoFormat(strFormat, objArgs)));
        }
        public static void Warn(object objMessage)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Warn(objMessage)));
        }
        public static void Warn(object objMessage, Exception objException)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Warn(objMessage, objException)));
        }
        public static void WarnFormat(string strFormat, params object[] objArgs)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.WarnFormat(strFormat, objArgs)));
        }
        public static void Debug(object objMessage)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Debug(objMessage)));
        }
        public static void Debug(object objMessage, Exception objException)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Debug(objMessage, objException)));
        }
        public static void DebugFormat(string strFormat, params object[] objArgs)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.DebugFormat(strFormat, objArgs)));
        }
        public static void Error(object objMessage)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Error(objMessage)));
        }
        public static void Error(object objMessage, Exception objException)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Error(objMessage, objException)));
        }
        public static void ErrorFormat(string strFormat, params object[] objArgs)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.ErrorFormat(strFormat, objArgs)));
        }
        public static void Fatal(object objMessage)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Fatal(objMessage)));
        }
        public static void Fatal(object objMessage, Exception objException)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.Fatal(objMessage, objException)));
        }
        public static void FatalFormat(string strFormat, params object[] objArgs)
        {
            AppRuntime.Instance.objLoggerList.ForEach(logger => Task.Run(() => logger.FatalFormat(strFormat, objArgs)));
        }
        #endregion

        #region 容器方法
        public static void Register(Action<IAbsfRegistration> objRegisterBuilder)
        {
            AppRuntime.Instance.objContainer.Register(objRegisterBuilder);
        }
        public static void RegisterType(Type objServiceType, Type objComponentType)
        {
            AppRuntime.Instance.objContainer.Register(f => f.RegisterType(objServiceType, objComponentType));
        }
        public static void RegisterType(Type objServiceType, Type objComponentType, string strName)
        {
            AppRuntime.Instance.objContainer.Register(f => f.RegisterType(objServiceType, objComponentType).Named(strName, objServiceType));
        }
        public static void RegisterType(Type objServiceType, Type objComponentType, LifetimeStyle iLifetimeStyle)
        {
            AppRuntime.Instance.objContainer.Register(f => f.RegisterType(objServiceType, objComponentType).Lifetime(iLifetimeStyle));
        }
        public static void RegisterType(Type objServiceType, Type objComponentType, string strName, LifetimeStyle iLifetimeStyle)
        {
            AppRuntime.Instance.objContainer.Register(f => f.RegisterType(objServiceType, objComponentType).Named(strName, objServiceType).Lifetime(iLifetimeStyle));
        }
        public static void RegisterType<TService>() where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterType<TService>();
        }
        public static void RegisterType<TService>(string strName) where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterType<TService>(strName);
        }
        public static void RegisterType<TService>(LifetimeStyle iLifetimeStyle) where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterType<TService>(iLifetimeStyle);
        }
        public static void RegisterType<TService>(string strName, LifetimeStyle iLifetimeStyle) where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterType<TService>(strName, iLifetimeStyle);
        }
        public static void RegisterType<TService, TComponent>()
            where TService : class
            where TComponent : class, TService
        {
            AppRuntime.Instance.objContainer.RegisterType<TService, TComponent>();
        }
        public static void RegisterType<TService, TComponent>(string strName)
            where TService : class
            where TComponent : class, TService
        {
            AppRuntime.Instance.objContainer.RegisterType<TService, TComponent>(strName);
        }
        public static void RegisterType<TService, TComponent>(LifetimeStyle iLifetimeStyle)
            where TService : class
            where TComponent : class, TService
        {
            AppRuntime.Instance.objContainer.RegisterType<TService, TComponent>(iLifetimeStyle);
        }
        public static void RegisterType<TService, TComponent>(string strName, LifetimeStyle iLifetimeStyle)
            where TService : class
            where TComponent : class, TService
        {
            AppRuntime.Instance.objContainer.RegisterType<TService, TComponent>(strName, iLifetimeStyle);
        }
        public static void RegisterInstance<TService>(TService objInstance) where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterInstance<TService>(objInstance);
        }
        public static void RegisterInstance<TService>(TService objInstance, string strName) where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterInstance<TService>(objInstance, strName);
        }
        public static void RegisterInstance<TService>(TService objInstance, LifetimeStyle iLifetimeStyle) where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterInstance<TService>(objInstance, iLifetimeStyle);
        }
        public static void RegisterInstance<TService>(TService objInstance, string strName, LifetimeStyle iLifetimeStyle) where TService : class
        {
            AppRuntime.Instance.objContainer.RegisterInstance<TService>(objInstance, strName, iLifetimeStyle);
        }
        public static void RegisterGeneric(Type objService, Type objComponent)
        {
            AppRuntime.Instance.objContainer.RegisterGeneric(objService, objComponent);
        }
        public static void RegisterGeneric(Type objService, Type objComponent, LifetimeStyle iLifetimeStyle)
        {
            AppRuntime.Instance.objContainer.RegisterGeneric(objService, objComponent, iLifetimeStyle);
        }
        public static object Resolve(Type objServiceType)
        {
            return AppRuntime.Instance.objContainer.Resolve(objServiceType);
        }
        public static object Resolve(Type objServiceType, params object[] objArgs)
        {
            return AppRuntime.Instance.objContainer.Resolve(objServiceType, objArgs);
        }
        public static object Resolve(Type objServiceType, IDictionary<string, object> objArgs)
        {
            return AppRuntime.Instance.objContainer.Resolve(objServiceType, objArgs);
        }
        public static TService Resolve<TService>(params object[] objArgs) where TService : class
        {
            return AppRuntime.Instance.objContainer.Resolve<TService>(objArgs);
        }
        public static TService Resolve<TService>(IDictionary<string, object> objArgs) where TService : class
        {
            return AppRuntime.Instance.objContainer.Resolve<TService>(objArgs);
        }
        public static TService ResolveNamed<TService>(string strName, params object[] objArgs) where TService : class
        {
            return AppRuntime.Instance.objContainer.ResolveNamed<TService>(strName, objArgs);
        }
        public static TService ResolveNamed<TService>(string strName, IDictionary<string, object> objArgs) where TService : class
        {
            return AppRuntime.Instance.objContainer.ResolveNamed<TService>(strName, objArgs);
        }
        public static IEnumerable<object> ResolveAll(Type objServiceType)
        {
            return AppRuntime.Instance.objContainer.ResolveAll(objServiceType);
        }
        public static IEnumerable<TService> ResolveAll<TService>() where TService : class
        {
            return AppRuntime.Instance.objContainer.ResolveAll<TService>();
        }
        public static bool HasRegister<TService>()
        {
            return AppRuntime.Instance.objContainer.HasRegister<TService>();
        }
        public static bool HasRegister(Type objType)
        {
            return AppRuntime.Instance.objContainer.HasRegister(objType);
        }
        public static bool HasRegister(string strName, Type objType)
        {
            return AppRuntime.Instance.objContainer.HasRegister(strName, objType);
        }
        public static bool HasRegister<TService>(string strName)
        {
            return AppRuntime.Instance.objContainer.HasRegister<TService>(strName);
        }
        #endregion
    }
}

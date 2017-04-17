using Basf.Logging;
using System;

namespace Basf
{
    public sealed class AppRuntime
    {
        #region 私有字段
        private static AppRuntime Instance = new AppRuntime();
        private IObjectContainer objContainer = null;

        //private List<InterceptorMatcher>
        #endregion

        #region 构造方法
        static AppRuntime()
        {
        }
        private AppRuntime()
        {
        }
        #endregion

        public static IObjectContainer Container => AppRuntime.Instance.objContainer;
        public static ILogger Logger = (ILogger)AppRuntime.Container?.Resolve(typeof(ILogger));

        #region 配置方法
        public static void Configure(Action<AppRuntime> objBuilder)
        {
            objBuilder?.Invoke(AppRuntime.Instance);
        }
        //public AppRuntime MatchInterceptor(InterceptorMatchKind kind, string expression, Type interceptorType)
        //{
        //    if (objContainer == null) throw new ArgumentNullException("objContainer");
        //    AppRuntime.Container = objContainer;
        //    return AppRuntime.Instance;
        //}
        //public AppRuntime MatchInterceptor(InterceptorMatchKind kind, string expression, string interceptorName)
        //{
        //    if (objContainer == null) throw new ArgumentNullException("Container");
        //    AppRuntime.Container = objContainer;
        //    return AppRuntime.Instance;
        //}
        public AppRuntime UsingContainer(IObjectContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            AppRuntime.Instance.objContainer = container;
            return AppRuntime.Instance;
        }
        public AppRuntime UsingLogger<TLogger>() where TLogger : class, ILogger
        {
            if (AppRuntime.Container == null) throw new ArgumentNullException("AppRuntime.Container");
            AppRuntime.Container.RegisterType<ILogger, TLogger>(LifetimeStyle.Singleton);
            return AppRuntime.Instance;
        }
        #endregion
    }
}

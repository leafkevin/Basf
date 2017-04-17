using System;
using System.Collections.Generic;

namespace Basf
{
    public interface IObjectContainer : IDisposable
    {
        #region ServiceRegistry组件注册
        void Register(Action<IAbsfRegistration> objRegisterBuilder);
        #endregion

        #region ServiceFetcher组件获取
        object Resolve(Type objServiceType, params object[] objArgs);
        object Resolve(Type objServiceType, IDictionary<string, object> objArgs);
        TService ResolveNamed<TService>(string strName, params object[] objArgs) where TService : class;
        TService ResolveNamed<TService>(string strName, IDictionary<string, object> objArgs) where TService : class;
        IEnumerable<object> ResolveAll(Type objServiceType);
        IEnumerable<TService> ResolveAll<TService>() where TService : class;
        #endregion

        #region Other
        bool HasRegister(Type objServiceType, string strName = null);
        #endregion
    }
}

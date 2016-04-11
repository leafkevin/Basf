using System;
using System.Collections.Generic;

namespace Basf
{
    public interface IObjectContainer
    {
        #region ServiceRegistry组件注册
        void Register(Action<IAbsfRegistration> objRegisterBuilder);
        #endregion

        #region ServiceFetcher组件获取
        object Resolve(Type objServiceType, params object[] objArgs);
        object Resolve(Type objServiceType, IDictionary<string, object> objArgs);
        TService Resolve<TService>(params object[] objArgs) where TService : class;
        TService Resolve<TService>(IDictionary<string, object> objArgs) where TService : class;
        TService ResolveNamed<TService>(string strName, params object[] objArgs) where TService : class;
        TService ResolveNamed<TService>(string strName, IDictionary<string, object> objArgs) where TService : class;
        IEnumerable<object> ResolveAll(Type objServiceType);
        IEnumerable<TService> ResolveAll<TService>() where TService : class;
        #endregion

        #region Other
        bool HasRegister(Type objType);
        bool HasRegister(string strName, Type objServiceType);
        bool HasRegister<TService>();
        bool HasRegister<TService>(string strName);
        #endregion
    }
}

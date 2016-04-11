using System;
using System.Collections.Generic;

namespace Basf
{
    public interface IObjectContainer
    {
        #region ServiceRegistry组件注册
        void Register(Action<IAbsfRegistration> objRegisterHandler);
        #endregion

        #region ServiceFetcher组件获取
        TService Resolve<TService>() where TService : class;
        TService Resolve<TService>(params object[] objArgs) where TService : class;
        TService Resolve<TService>(IDictionary<string, object> objArgs) where TService : class;
        TService ResolveNamed<TService>(string strName) where TService : class;
        TService ResolveNamed<TService>(string strName, params object[] objArgs) where TService : class;
        TService ResolveNamed<TService>(string strName, IDictionary<string, object> objArgs) where TService : class;
        #endregion

        #region Other
        bool HasRegister(string strName, Type objType);
        bool HasRegister(Type objType);
        bool HasRegister<TService>(string strName);
        bool HasRegister<TService>();
        #endregion
    }
}

using Autofac;
using System;
using System.Collections.Generic;

namespace Basf.Autofac
{
    public class AutofacContainer : IObjectContainer
    {
        private readonly IContainer objContainer;
        public AutofacContainer()
        {
            objContainer = new ContainerBuilder().Build();
        }
        public AutofacContainer(ContainerBuilder objContainerBuilder)
        {
            objContainer = objContainerBuilder.Build();
        }
        public IContainer Container
        {
            get
            {
                return objContainer;
            }
        }
        public void Register(Action<IAbsfRegistration> objRegisterHandler)
        {
            IAbsfRegistration objRegistration = new AbsfRegistration(this.objContainer);
            if (objRegisterHandler != null)
            {
                objRegisterHandler.Invoke(objRegistration);
            }
        }
        public object Resolve(Type objServiceType, params object[] objArgs)
        {
            Utility.Fail(objServiceType == null, "参数objServiceType不能为空！");
            if (objArgs != null)
            {
                return this.objContainer.Resolve(objServiceType, this.ParmaeterList(objArgs));
            }
            return this.objContainer.Resolve(objServiceType);
        }
        public object Resolve(Type objServiceType, IDictionary<string, object> objArgs)
        {
            Utility.Fail(objServiceType == null, "参数objServiceType不能为空！");
            Utility.Fail(objArgs == null, "参数objArgs不能为空！");
            return this.objContainer.Resolve(objServiceType, this.ParmaeterList(objArgs));
        }
        public TService Resolve<TService>(params object[] objArgs) where TService : class
        {
            if (objArgs != null)
            {
                return this.objContainer.Resolve<TService>(this.ParmaeterList(objArgs));
            }
            else
            {
                return this.objContainer.Resolve<TService>();
            }
        }
        public TService Resolve<TService>(IDictionary<string, object> objArgs) where TService : class
        {
            Utility.Fail(objArgs == null, "参数objArgs不能为空！");
            return this.objContainer.Resolve<TService>(this.ParmaeterList(objArgs));
        }
        public TService ResolveNamed<TService>(string strName, params object[] objArgs) where TService : class
        {
            Utility.Fail(String.IsNullOrEmpty(strName), "参数strName不能为空！");
            if (objArgs != null)
            {
                return this.objContainer.ResolveNamed<TService>(strName, this.ParmaeterList(objArgs));
            }
            else
            {
                return this.objContainer.ResolveNamed<TService>(strName);
            }
        }
        public TService ResolveNamed<TService>(string strName, IDictionary<string, object> objArgs) where TService : class
        {
            Utility.Fail(String.IsNullOrEmpty(strName), "参数strName不能为空！");
            Utility.Fail(objArgs == null, "参数objArgs不能为空！");
            return this.objContainer.ResolveNamed<TService>(strName, this.ParmaeterList(objArgs));
        }
        public IEnumerable<object> ResolveAll(Type objServiceType)
        {
            Utility.Fail(objServiceType == null, "参数objServiceType不能为空！");
            Type objType = typeof(IEnumerable<>).MakeGenericType(objServiceType);
            return (IEnumerable<object>)this.objContainer.Resolve(objType);
        }
        public IEnumerable<TService> ResolveAll<TService>() where TService : class
        {
            return this.objContainer.Resolve<IEnumerable<TService>>();
        }
        public bool HasRegister(Type objServiceType)
        {
            Utility.Fail(objServiceType == null, "参数objServiceType不能为空！");
            return this.objContainer.IsRegistered(objServiceType);
        }
        public bool HasRegister(string strName, Type objServiceType)
        {
            Utility.Fail(String.IsNullOrEmpty(strName), "参数strName不能为空！");
            Utility.Fail(objServiceType == null, "参数objServiceType不能为空！");
            return this.objContainer.IsRegisteredWithName(strName, objServiceType);
        }
        public bool HasRegister<TService>()
        {
            return this.objContainer.IsRegistered<TService>();
        }
        public bool HasRegister<TService>(string strName)
        {
            Utility.Fail(String.IsNullOrEmpty(strName), "参数strName不能为空！");
            return this.objContainer.IsRegisteredWithName<TService>(strName);
        }
        private List<NamedParameter> ParmaeterList(IDictionary<string, object> objArgs)
        {
            List<NamedParameter> objParameter = new List<NamedParameter>();
            foreach (KeyValuePair<string, object> objEntity in objArgs)
            {
                objParameter.Add(new NamedParameter(objEntity.Key, objEntity.Value));
            }
            return objParameter;
        }
        private List<PositionalParameter> ParmaeterList(params object[] objArgs)
        {
            List<PositionalParameter> objParameter = new List<PositionalParameter>();
            for (int i = 0; i < objArgs.Length; i++)
            {
                objParameter.Add(new PositionalParameter(i, objArgs[i]));
            }
            return objParameter;
        }
    }
}

using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;

namespace Basf.Autofac
{
    public class AutofacGenericExpression : IAbsfRegistrationGenericExpression
    {
        private IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> objRegistrationBuilder;
        private ContainerBuilder objBuilder = null;
        public AutofacGenericExpression(Type objComponent)
        {
            this.objBuilder = new ContainerBuilder();
            this.objRegistrationBuilder = this.objBuilder.RegisterGeneric(objComponent);
        }
        public AutofacGenericExpression(Type objServiceType, Type objComponentType)
        {
            this.objBuilder = new ContainerBuilder();
            this.objRegistrationBuilder = this.objBuilder.RegisterGeneric(objComponentType).As(objServiceType);
        }
        public IAbsfRegistrationGenericExpression Forward(params Type[] objServices)
        {
            this.objRegistrationBuilder.As(objServices);
            return this;
        }
        public IAbsfRegistrationGenericExpression Forward<TService>()
        {
            this.objRegistrationBuilder.As<TService>();
            return this;
        }
        public IAbsfRegistrationGenericExpression Forward<TService1, TService2>()
        {
            this.objRegistrationBuilder.As<TService1, TService2>();
            return this;
        }
        public IAbsfRegistrationGenericExpression Forward<TService1, TService2, TService3>()
        {
            this.objRegistrationBuilder.As<TService1, TService2, TService3>();
            return this;
        }
        public IAbsfRegistrationGenericExpression Named<TService>(string strName)
        {
            this.objRegistrationBuilder.Named<TService>(strName);
            return this;
        }
        public IAbsfRegistrationGenericExpression Named(string strName, Type objServiceType)
        {
            this.objRegistrationBuilder.Named(strName, objServiceType);
            return this;
        }
        public IAbsfRegistrationGenericExpression Lifetime(LifetimeStyle iLifetimeStyle)
        {
            switch (iLifetimeStyle)
            {
                case LifetimeStyle.Singleton:
                    {
                        this.objRegistrationBuilder.SingleInstance();
                        break;
                    }
                case LifetimeStyle.Thread:
                    {
                        this.objRegistrationBuilder.InstancePerLifetimeScope();
                        break;
                    }
                case LifetimeStyle.Request:
                    {
                        this.objRegistrationBuilder.InstancePerRequest();
                        break;
                    }
                case LifetimeStyle.Pool:
                case LifetimeStyle.Transient:
                    {
                        this.objRegistrationBuilder.InstancePerDependency();
                        break;
                    }
            }
            return this;
        }
        public IAbsfRegistrationGenericExpression OwnedLifetime(Type objServiceType)
        {
            this.objRegistrationBuilder.InstancePerOwned(objServiceType);
            return this;
        }
        public IAbsfRegistrationGenericExpression OwnedLifetime<TService>()
        {
            this.objRegistrationBuilder.InstancePerOwned<TService>();
            return this;
        }
        public IAbsfRegistrationGenericExpression UsingConstructor(params Type[] objTypes)
        {
            this.objRegistrationBuilder.UsingConstructor(objTypes);
            return this;
        }
        public IAbsfRegistrationGenericExpression WithParameter(string strName, object objValue)
        {
            this.objRegistrationBuilder.WithParameter(strName, objValue);
            return this;
        }
        public IAbsfRegistrationGenericExpression WithParameters(params object[] objParamters)
        {
            List<PositionalParameter> objParameterList = null;
            if (objParamters != null && objParamters.Length > 0)
            {
                objParameterList = new List<PositionalParameter>();
                for (int i = 0; i < objParamters.Length; i++)
                {
                    objParameterList.Add(new PositionalParameter(i, objParamters[i]));
                }
            }
            this.objRegistrationBuilder.WithParameters(objParameterList);
            return this;
        }
        public IAbsfRegistrationGenericExpression WithParameters(IDictionary<string, object> objParamters)
        {
            List<NamedParameter> objParameterList = null;
            if (objParamters != null && objParamters.Count > 0)
            {
                objParameterList = new List<NamedParameter>();
                foreach (KeyValuePair<string, object> objEntity in objParamters)
                {
                    objParameterList.Add(new NamedParameter(objEntity.Key, objEntity.Value));
                }
            }
            this.objRegistrationBuilder.WithParameters(objParameterList);
            return this;
        }
        public IAbsfRegistrationGenericExpression WithInterfaceInterceptor(Type interceptorType)
        {
            this.objRegistrationBuilder.EnableInterfaceInterceptors().InterceptedBy(interceptorType);
            return this;
        }
        public void Update(IContainer objContainer)
        {
            this.objBuilder.Update(objContainer);
        }
    }
}

using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;

namespace Basf.Autofac
{
    public class AutofacTypeExpression : IAbsfRegistrationTypeExpression
    {
        private ContainerBuilder objBuilder = null;
        private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> objRegistrationBuilder;
        public AutofacTypeExpression(Type objComponentType)
        {
            this.objBuilder = new ContainerBuilder();
            this.objRegistrationBuilder = this.objBuilder.RegisterType(objComponentType);
        }
        public AutofacTypeExpression(Type objServiceType, Type objComponentType)
        {
            this.objBuilder = new ContainerBuilder();
            this.objRegistrationBuilder = this.objBuilder.RegisterType(objComponentType).As(objServiceType);
        }
        public IAbsfRegistrationTypeExpression Forward(params Type[] objServices)
        {
            this.objRegistrationBuilder.As(objServices);
            return this;
        }
        public IAbsfRegistrationTypeExpression Forward<TService>()
        {
            this.objRegistrationBuilder.As<TService>();
            return this;
        }
        public IAbsfRegistrationTypeExpression Forward<TService1, TService2>()
        {
            this.objRegistrationBuilder.As<TService1, TService2>();
            return this;
        }
        public IAbsfRegistrationTypeExpression Forward<TService1, TService2, TService3>()
        {
            this.objRegistrationBuilder.As<TService1, TService2, TService3>();
            return this;
        }
        public IAbsfRegistrationTypeExpression Named<TService>(string strName)
        {
            this.objRegistrationBuilder.Named<TService>(strName);
            return this;
        }
        public IAbsfRegistrationTypeExpression Named(string strName, Type objServiceType)
        {
            this.objRegistrationBuilder.Named(strName, objServiceType);
            return this;
        }
        public IAbsfRegistrationTypeExpression Lifetime(LifetimeStyle iLifetimeStyle)
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
        public IAbsfRegistrationTypeExpression OwnedLifetime(Type objServiceType)
        {
            this.objRegistrationBuilder.InstancePerOwned(objServiceType);
            return this;
        }
        public IAbsfRegistrationTypeExpression OwnedLifetime<TService>()
        {
            this.objRegistrationBuilder.InstancePerOwned<TService>();
            return this;
        }
        public IAbsfRegistrationTypeExpression UsingConstructor(params Type[] objTypes)
        {
            this.objRegistrationBuilder.UsingConstructor(objTypes);
            return this;
        }
        public IAbsfRegistrationTypeExpression WithParameter(string strName, object objValue)
        {
            this.objRegistrationBuilder.WithParameter(strName, objValue);
            return this;
        }
        public IAbsfRegistrationTypeExpression WithParameters(params object[] objParamters)
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
        public IAbsfRegistrationTypeExpression WithParameters(IDictionary<string, object> objParamters)
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
        public IAbsfRegistrationTypeExpression WithInterfaceInterceptor(Type interceptorType)
        {
            this.objRegistrationBuilder.EnableInterfaceInterceptors().InterceptedBy(interceptorType);
            return this;
        }
        public IAbsfRegistrationTypeExpression WithClassInterceptor(Type interceptorType)
        {
            this.objRegistrationBuilder.EnableClassInterceptors().InterceptedBy(interceptorType);
            return this;
        }
        public void Update(IContainer objContainer)
        {
            this.objBuilder.Update(objContainer);
        }
    }
    public class AutofacTypeExpression<TComponent> : IAbsfRegistrationTypeExpression<TComponent> where TComponent : class
    {
        private ContainerBuilder objBuilder = null;
        private IRegistrationBuilder<TComponent, ConcreteReflectionActivatorData, SingleRegistrationStyle> objRegistrationBuilder;
        public AutofacTypeExpression()
        {
            this.objBuilder = new ContainerBuilder();
            this.objRegistrationBuilder = this.objBuilder.RegisterType<TComponent>();
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward(params Type[] objServices)
        {
            this.objRegistrationBuilder.As(objServices);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward<TService>()
        {
            this.objRegistrationBuilder.As<TService>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward<TService1, TService2>()
        {
            this.objRegistrationBuilder.As<TService1, TService2>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward<TService1, TService2, TService3>()
        {
            this.objRegistrationBuilder.As<TService1, TService2, TService3>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Named<TService>(string strName)
        {
            this.objRegistrationBuilder.Named<TService>(strName);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Named(string strName, Type objServiceType)
        {
            this.objRegistrationBuilder.Named(strName, objServiceType);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Lifetime(LifetimeStyle iLifetimeStyle)
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
        public IAbsfRegistrationTypeExpression<TComponent> OwnedLifetime(Type objServiceType)
        {
            this.objRegistrationBuilder.InstancePerOwned(objServiceType);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> OwnedLifetime<TService>()
        {
            this.objRegistrationBuilder.InstancePerOwned<TService>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> UsingConstructor(params Type[] objTypes)
        {
            this.objRegistrationBuilder.UsingConstructor(objTypes);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> WithParameter(string strName, object objValue)
        {
            this.objRegistrationBuilder.WithParameter(strName, objValue);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> WithParameters(params object[] objParamters)
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
        public IAbsfRegistrationTypeExpression<TComponent> WithParameters(IDictionary<string, object> objParamters)
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
        public IAbsfRegistrationTypeExpression<TComponent> WithInterfaceInterceptor(Type interceptorType)
        {
            this.objRegistrationBuilder.EnableInterfaceInterceptors().InterceptedBy(interceptorType);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> WithClassInterceptor(Type interceptorType)
        {
            this.objRegistrationBuilder.EnableClassInterceptors().InterceptedBy(interceptorType);
            return this;
        }
        public void Update(IContainer objContainer)
        {
            this.objBuilder.Update(objContainer);
        }
    }
    public class AutofacTypeExpression<TService, TComponent> : IAbsfRegistrationTypeExpression<TComponent>
        where TService : class
        where TComponent : class, TService
    {
        private ContainerBuilder objBuilder = null;
        private IRegistrationBuilder<TComponent, ConcreteReflectionActivatorData, SingleRegistrationStyle> objRegistrationBuilder;
        public AutofacTypeExpression()
        {
            this.objBuilder = new ContainerBuilder();
            this.objRegistrationBuilder = this.objBuilder.RegisterType<TComponent>().As<TService>();
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward(params Type[] objServices)
        {
            this.objRegistrationBuilder.As(objServices);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward<TServiceType>()
        {
            this.objRegistrationBuilder.As<TServiceType>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward<TService1, TService2>()
        {
            this.objRegistrationBuilder.As<TService1, TService2>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Forward<TService1, TService2, TService3>()
        {
            this.objRegistrationBuilder.As<TService1, TService2, TService3>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Named<TServiceType>(string strName)
        {
            this.objRegistrationBuilder.Named<TServiceType>(strName);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Named(string strName, Type objServiceType)
        {
            this.objRegistrationBuilder.Named(strName, objServiceType);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> Lifetime(LifetimeStyle iLifetimeStyle)
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
        public IAbsfRegistrationTypeExpression<TComponent> OwnedLifetime(Type objServiceType)
        {
            this.objRegistrationBuilder.InstancePerOwned(objServiceType);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> OwnedLifetime<TServiceType>()
        {
            this.objRegistrationBuilder.InstancePerOwned<TServiceType>();
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> UsingConstructor(params Type[] objTypes)
        {
            this.objRegistrationBuilder.UsingConstructor(objTypes);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> WithParameter(string strName, object objValue)
        {
            this.objRegistrationBuilder.WithParameter(strName, objValue);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> WithParameters(params object[] objParamters)
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
        public IAbsfRegistrationTypeExpression<TComponent> WithParameters(IDictionary<string, object> objParamters)
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
        public IAbsfRegistrationTypeExpression<TComponent> WithInterfaceInterceptor(Type interceptorType)
        {
            this.objRegistrationBuilder.EnableInterfaceInterceptors().InterceptedBy(interceptorType);
            return this;
        }
        public IAbsfRegistrationTypeExpression<TComponent> WithClassInterceptor(Type interceptorType)
        {
            this.objRegistrationBuilder.EnableClassInterceptors().InterceptedBy(interceptorType);
            return this;
        }
        public void Update(IContainer objContainer)
        {
            this.objBuilder.Update(objContainer);
        }
    }
}

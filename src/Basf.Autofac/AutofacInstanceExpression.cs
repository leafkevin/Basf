using Autofac;
using Autofac.Builder;
using System;
using System.Collections.Generic;

namespace Basf.Autofac
{
    public class AutofacInstanceExpression<TComponent> : IAbsfRegistrationInstanceExpression<TComponent> where TComponent : class
    {
        private IRegistrationBuilder<TComponent, SimpleActivatorData, SingleRegistrationStyle> objRegistrationBuilder;
        private ContainerBuilder objBuilder = null;
        public AutofacInstanceExpression(TComponent objInstance)
        {
            this.objBuilder = new ContainerBuilder();
            this.objRegistrationBuilder = this.objBuilder.RegisterInstance<TComponent>(objInstance);
        }
        public IAbsfRegistrationInstanceExpression<TComponent> Forward(params Type[] objServices)
        {
            this.objRegistrationBuilder.As(objServices);
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> Forward<TService>()
        {
            this.objRegistrationBuilder.As<TService>();
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> Forward<TService1, TService2>()
        {
            this.objRegistrationBuilder.As<TService1, TService2>();
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> Forward<TService1, TService2, TService3>()
        {
            this.objRegistrationBuilder.As<TService1, TService2, TService3>();
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> Named<TService>(string strName)
        {
            this.objRegistrationBuilder.Named<TService>(strName);
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> Named(string strName, Type objServiceType)
        {
            this.objRegistrationBuilder.Named(strName, objServiceType);
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> Lifetime(LifetimeStyle iLifetimeStyle)
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
                    {
                        this.objRegistrationBuilder.InstancePerDependency();
                        break;
                    }
                case LifetimeStyle.Transient:
                    {
                        this.objRegistrationBuilder.InstancePerDependency();
                        break;
                    }
            }
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> OwnedLifetime(Type objServiceType)
        {
            this.objRegistrationBuilder.InstancePerOwned(objServiceType);
            return this;
        }
        public IAbsfRegistrationInstanceExpression<TComponent> OwnedLifetime<TService>()
        {
            this.objRegistrationBuilder.InstancePerOwned<TService>();
            return this;
        }
        public void Update(IContainer objContainer)
        {
            this.objBuilder.Update(objContainer);
        }
    }
}

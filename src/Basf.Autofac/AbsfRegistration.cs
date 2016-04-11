using Autofac;
using System;

namespace Basf.Autofac
{
    public class AbsfRegistration : IAbsfRegistration
    {
        private IContainer objContainer = null;
        public AbsfRegistration(IContainer objContainer)
        {
            this.objContainer = objContainer;
        }
        public IAbsfRegistrationTypeExpression<TService> RegisterType<TService>() where TService : class
        {
            AutofacTypeExpression<TService> objAdapter = new AutofacTypeExpression<TService>();
            objAdapter.Update(this.objContainer);
            return objAdapter;
        }
        public IAbsfRegistrationTypeExpression RegisterType(Type objServiceType)
        {
            AutofacTypeExpression objAdapter = new AutofacTypeExpression(objServiceType);
            objAdapter.Update(this.objContainer);
            return objAdapter;
        }
        public IAbsfRegistrationInstanceExpression<TService> RegisterType<TService>(TService objInstance) where TService : class
        {
            AutofacInstanceExpression<TService> objAdapter = new AutofacInstanceExpression<TService>(objInstance);
            objAdapter.Update(this.objContainer);
            return objAdapter;
        }
        public IAbsfRegistrationTypeExpression<TComponent> RegisterType<TService, TComponent>()
            where TService : class
            where TComponent : class, TService
        {
            AutofacTypeExpression<TService, TComponent> objAdapter = new AutofacTypeExpression<TService, TComponent>();
            objAdapter.Update(this.objContainer);
            return objAdapter;
        }
        public IAbsfRegistrationTypeExpression RegisterType(Type objServiceType, Type objComponentType)
        {
            AutofacTypeExpression objAdapter = new AutofacTypeExpression(objServiceType, objComponentType);
            objAdapter.Update(this.objContainer);
            return objAdapter;
        }
        public IAbsfRegistrationGenericExpression RegisterGeneric(Type objServiceType, Type objComponentType)
        {
            AutofacGenericExpression objAdapter = new AutofacGenericExpression(objServiceType, objComponentType);
            objAdapter.Update(this.objContainer);
            return objAdapter;
        }
    }
}

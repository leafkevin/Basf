using System;
using System.Collections.Generic;

namespace Basf
{
    public interface IAbsfRegistration
    {
        IAbsfRegistrationTypeExpression RegisterType(Type objServiceType, Type objComponentType);
        IAbsfRegistrationTypeExpression<TService> RegisterType<TService>() where TService : class;
        IAbsfRegistrationTypeExpression RegisterType(Type objServiceType);
        IAbsfRegistrationInstanceExpression<TService> RegisterType<TService>(TService objInstance) where TService : class;
        IAbsfRegistrationTypeExpression<TComponent> RegisterType<TService, TComponent>()
            where TService : class
            where TComponent : class, TService;       
        IAbsfRegistrationGenericExpression RegisterGeneric(Type objService, Type objComponent);
    }
    public interface IAbsfRegistrationTypeExpression
    {
        IAbsfRegistrationTypeExpression Forward<TService>();
        IAbsfRegistrationTypeExpression Forward<TService1, TService2>();
        IAbsfRegistrationTypeExpression Forward<TService1, TService2, TService3>();
        IAbsfRegistrationTypeExpression Forward(params Type[] objServiceTypes);
        IAbsfRegistrationTypeExpression Named<TService>(string strName);
        IAbsfRegistrationTypeExpression Named(string strName, Type objServiceType);
        IAbsfRegistrationTypeExpression Lifetime(LifetimeStyle iLifetimeStyle);
        IAbsfRegistrationTypeExpression OwnedLifetime(Type objServiceType);
        IAbsfRegistrationTypeExpression OwnedLifetime<TService>();
        IAbsfRegistrationTypeExpression UsingConstructor(params Type[] objTypes);
        IAbsfRegistrationTypeExpression WithParameter(string strName, object objValue);
        IAbsfRegistrationTypeExpression WithParameters(params object[] objParamters);
        IAbsfRegistrationTypeExpression WithParameters(IDictionary<string, object> objParamters);
    }
    public interface IAbsfRegistrationTypeExpression<TComponent>
    {
        IAbsfRegistrationTypeExpression<TComponent> Forward<TService>();
        IAbsfRegistrationTypeExpression<TComponent> Forward<TService1, TService2>();
        IAbsfRegistrationTypeExpression<TComponent> Forward<TService1, TService2, TService3>();
        IAbsfRegistrationTypeExpression<TComponent> Forward(params Type[] objServiceTypes);
        IAbsfRegistrationTypeExpression<TComponent> Named<TService>(string strName);
        IAbsfRegistrationTypeExpression<TComponent> Named(string strName, Type objServiceType);
        IAbsfRegistrationTypeExpression<TComponent> Lifetime(LifetimeStyle iLifetimeStyle);
        IAbsfRegistrationTypeExpression<TComponent> OwnedLifetime(Type objServiceType);
        IAbsfRegistrationTypeExpression<TComponent> OwnedLifetime<TService>();
        IAbsfRegistrationTypeExpression<TComponent> UsingConstructor(params Type[] objTypes);
        IAbsfRegistrationTypeExpression<TComponent> WithParameter(string strName, object objValue);
        IAbsfRegistrationTypeExpression<TComponent> WithParameters(params object[] objParamters);
        IAbsfRegistrationTypeExpression<TComponent> WithParameters(IDictionary<string, object> objParamters);
    }
    public interface IAbsfRegistrationInstanceExpression<TComponent>
    {
        IAbsfRegistrationInstanceExpression<TComponent> Forward<TService>();
        IAbsfRegistrationInstanceExpression<TComponent> Forward<TService1, TService2>();
        IAbsfRegistrationInstanceExpression<TComponent> Forward<TService1, TService2, TService3>();
        IAbsfRegistrationInstanceExpression<TComponent> Forward(params Type[] objServiceTypes);
        IAbsfRegistrationInstanceExpression<TComponent> Named<TService>(string strName);
        IAbsfRegistrationInstanceExpression<TComponent> Named(string strName, Type objServiceType);
        IAbsfRegistrationInstanceExpression<TComponent> Lifetime(LifetimeStyle iLifetimeStyle);
        IAbsfRegistrationInstanceExpression<TComponent> OwnedLifetime(Type objServiceType);
        IAbsfRegistrationInstanceExpression<TComponent> OwnedLifetime<TService>();
    }

    public interface IAbsfRegistrationGenericExpression
    {
        IAbsfRegistrationGenericExpression Forward<TService>();
        IAbsfRegistrationGenericExpression Forward<TService1, TService2>();
        IAbsfRegistrationGenericExpression Forward<TService1, TService2, TService3>();
        IAbsfRegistrationGenericExpression Forward(params Type[] objServiceTypes);
        IAbsfRegistrationGenericExpression Lifetime(LifetimeStyle iLifetimeStyle);
        IAbsfRegistrationGenericExpression OwnedLifetime(Type objServiceType);
        IAbsfRegistrationGenericExpression OwnedLifetime<TService>();
        IAbsfRegistrationGenericExpression UsingConstructor(params Type[] objTypes);
        IAbsfRegistrationGenericExpression WithParameter(string strName, object objValue);
        IAbsfRegistrationGenericExpression WithParameters(params object[] objParamters);
        IAbsfRegistrationGenericExpression WithParameters(IDictionary<string, object> objParamters);
    }
}

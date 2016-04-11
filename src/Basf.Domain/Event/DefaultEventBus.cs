using System;

namespace Basf.Domain.Event
{
    public class DefaultEventBus : IEventBus
    {
        public void Send<TAggRootId>(IDomainEvent<TAggRootId> domainEvent)
        {
            try
            {
                Type type = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType().GenericTypeArguments[0]);
                IEventHandler<TAggRootId> handler = AppRuntime.Resolve(type) as IEventHandler<TAggRootId>;
                handler.Handle(type, domainEvent);
            }
            catch (Exception ex)
            {
                AppRuntime.ErrorFormat("领域事件{0}执行失败。Exception:{1}", domainEvent, ex.ToString());
            }
        }
        public void Send<TEvent, TAggRootId>(TEvent domainEvent) where TEvent : class, IDomainEvent<TAggRootId>
        {
            try
            {
                AppRuntime.Resolve<IEventHandler<TEvent, TAggRootId>>().Handle(domainEvent);
            }
            catch (Exception ex)
            {
                AppRuntime.ErrorFormat("领域事件{0}执行失败。Exception:{1}", domainEvent, ex.ToString());
            }
        }
    }
}

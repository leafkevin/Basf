using System;

namespace Basf.Domain.Event
{
    public interface IEventHandler<TAggRootId>
    {
        void Handle(Type eventType, IDomainEvent<TAggRootId> domainEvent);
    }
    public interface IEventHandler<TEvent, TAggRootId> : IEventHandler<TAggRootId> where TEvent : class, IDomainEvent<TAggRootId>
    {
        void Handle(TEvent domainEvent);
    }
}

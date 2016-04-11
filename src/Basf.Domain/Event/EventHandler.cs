using System;
using Basf.Data;

namespace Basf.Domain.Event
{
    public abstract class EventHandler<TEvent, TAggRootId> : IEventHandler<TEvent, TAggRootId> where TEvent : class, IDomainEvent<TAggRootId>
    {
        public abstract void Handle(TEvent objEvent);
        public void Handle(Type eventType, IDomainEvent<TAggRootId> objEvent)
        {
            this.Handle(Convert.ChangeType(objEvent, eventType) as TEvent);
        }
        protected void Resolve(TEvent objEvent, EventResponse<TAggRootId> domainEventResult)
        {
            //发送Resolve消息 TODO
        }
        protected void Reject(IDomainEvent<TAggRootId> domainEvent, ActionResponse result)
        {
            //发送Reject消息 TODO
        }
    }
}

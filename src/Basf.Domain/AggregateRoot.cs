using System;
using System.Collections.Generic;
using Basf.Domain.Event;
using System.Linq;
using Basf.Domain.Storage;

namespace Basf.Domain
{
    [Serializable]
    public abstract class AggregateRoot<TAggRootId> : IAggregateRoot<TAggRootId>, IEventSourcing<TAggRootId>
    {
        private List<IDomainEvent<TAggRootId>> uncommitedEvents = new List<IDomainEvent<TAggRootId>>();
        public TAggRootId UniqueId { get; private set; }
        public int Version { get; private set; }
        public AggregateRoot(TAggRootId uniqueId)
        {
            this.UniqueId = uniqueId;
            this.Version = 0;
        }
        protected void ApplyChange<TEvent>(string commandId, TEvent domainEvent) where TEvent : DomainEvent<TAggRootId>
        {
            domainEvent.AggRootId = this.UniqueId;
            domainEvent.CommandId = commandId;
            domainEvent.Version = this.Version + 1;
            this.uncommitedEvents.Add(domainEvent);
            this.StoreEvent(domainEvent);
            var eventHandler = this as IEventHandler<TEvent, TAggRootId>;
            Utility.Fail(eventHandler == null, "聚合根类型{0}没有实现IEventHandler<{0},{1}>接口",
                this.GetType().Name, this.UniqueId.GetType().FullName);
            eventHandler?.Handle(domainEvent);
            this.Version = domainEvent.Version;
            this.uncommitedEvents.Clear();
            AppRuntime.Resolve<IEventBus>().Send<TEvent, TAggRootId>(domainEvent);
        }
        private void ApplyChange(IDomainEvent<TAggRootId> domainEvent)
        {
            var eventHandler = this as IEventHandler<TAggRootId>;
            Utility.Fail(eventHandler == null, "聚合根类型{0}没有实现IEventHandler<{0},{1}>接口",
                this.GetType().Name, this.UniqueId.GetType().FullName);
            eventHandler?.Handle(domainEvent.GetType(), domainEvent);
            this.Version = domainEvent.Version;
            AppRuntime.Resolve<IEventBus>().Send<TAggRootId>(domainEvent);
        }
        private void StoreEvent<TEvent>(TEvent domainEvent) where TEvent : DomainEvent<TAggRootId>
        {
            AppRuntime.Resolve<IEventStore>().Add<TEvent, TAggRootId>(domainEvent);
        }
        public void CreateSnapshot(int? version )
        {
            throw new NotImplementedException();
        }
        public void LoadFromSnapshot()
        {
            throw new NotImplementedException();
        }
        public void ReplayFrom(IEnumerable<IDomainEvent<TAggRootId>> domainEvents)
        {
            var eventArray = domainEvents.ToArray();
            for (int i = 0; i < eventArray.Length; i++)
            {
                this.ApplyChange(eventArray[i]);
                this.Version = eventArray[i].Version;
            }
        }
    }
}
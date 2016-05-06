using System;
using System.Collections.Generic;
using Basf.Domain.Event;
using System.Linq;
using Basf.Domain.Storage;
using System.Threading.Tasks;
using System.Reflection;

namespace Basf.Domain
{
    public abstract class AggRoot : IAggRoot, IEventSourcing
    {
        private List<IDomainEvent> uncommittedEvents = new List<IDomainEvent>();
        public string UniqueId { get; private set; }
        public int Version { get; private set; }
        public AggRoot(string uniqueId)
        {
            this.UniqueId = uniqueId;
            this.Version = 0;
        }       
        protected async Task ApplyChange(IDomainEvent domainEvent)
        {
            await AppRuntime.Resolve<IDomainContext>().PublishAsync(domainEvent);
        }
        protected async Task AcceptChange(IDomainEvent domainEvent)
        {
            if (this.VerifyEvent(domainEvent))
            {
                await this.HandleEvent(domainEvent);
            }
            else
            {
                if (!this.uncommittedEvents.Contains(domainEvent))
                {
                    this.uncommittedEvents.Add(domainEvent);
                    this.uncommittedEvents = this.uncommittedEvents.OrderBy(f => f.Version).ToList();
                    while (this.VerifyEvent(this.uncommittedEvents[0]))
                    {
                        await this.HandleEvent(domainEvent);
                        this.uncommittedEvents.RemoveAt(0);
                    }
                }
            }
        }
        protected bool VerifyEvent(IDomainEvent domainEvent)
        {
            return this.Version + 1 == domainEvent.Version;
        }
        public async Task CreateSnapshot()
        {
            await AppRuntime.Resolve<ISnapshotStore>().CreateAsync(this);
        }
        public async Task ReplayFrom(IEnumerable<IDomainEvent> domainEvents)
        {
            var eventArray = domainEvents.OrderBy(f => f.Version).ToArray();
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                await this.AcceptChange(domainEvent);
            }
        }
        public override string ToString()
        {
            return String.Format("{{AggRootType:{0},AggRootId:{1}}}", this.GetType().FullName, this.UniqueId);
        }
        private async Task HandleEvent(IDomainEvent domainEvent)
        {
            Type eventType = domainEvent.GetType();
            Type handlerType = this.GetType();
            var handler = AppRuntime.Resolve<IDomainContext>().GetEventHandler(eventType);
            if (handler == null)
            {
                handler = HandlerFactory.CreateActionHandler<IAggRoot, IDomainEvent>("Handle",
                    BindingFlags.Instance | BindingFlags.Public, eventType, handlerType);
                AppRuntime.Resolve<IDomainContext>().AddEventHandler(handlerType, eventType);
            }
            handler.Invoke(this, domainEvent);
            await AppRuntime.Resolve<IDomainContext>().PublishAsync(domainEvent);
            await AppRuntime.Resolve<IEventStore>().UpdateResultAsync(domainEvent, EventResult.Executed);
            this.Version = domainEvent.Version;
        }
    }
    [Serializable]
    public abstract class AggRoot<TAggRootId> : AggRoot
    {
        public new TAggRootId UniqueId { get; private set; }
        public AggRoot(TAggRootId uniqueId)
            : base(uniqueId.ToString())
        {
            this.UniqueId = uniqueId;
        }
    }
}
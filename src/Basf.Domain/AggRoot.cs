using System;
using System.Collections.Generic;
using Basf.Domain.Event;
using System.Linq;
using Basf.Domain.Storage;
using System.Threading.Tasks;
using Basf.Data;

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
        protected async Task<ActionResponse> ApplyChange(IDomainEvent domainEvent)
        {
            return await AppRuntime.Resolve<IDomainContext>().ApplyChange(domainEvent);
        }
        protected async Task<ActionResponse> AcceptChange(IDomainEvent domainEvent)
        {
            if (this.VerifyEvent(domainEvent))
            {
                return await this.HandleEvent(domainEvent);
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
                    if (!this.uncommittedEvents.Contains(domainEvent))
                    {
                        return ActionResponse.Success;
                    }
                }
                return ActionResponse.Fail((int)DomainError.EventExecuteAggRootVersionError, String.Format("事件执行异常，聚合根{0}版本Version：{1},最小事件版本Version：{2},当前事件版本Version：{3}",
                    this.ToString(), this.Version, this.uncommittedEvents[0].Version, domainEvent.Version));
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
        private async Task<ActionResponse> HandleEvent(IDomainEvent domainEvent)
        {
            var domainContext = AppRuntime.Resolve<IDomainContext>();
            var result = domainContext.InvokeHandler(this, domainEvent);
            if (result.Result == ActionResult.Failed)
            {
                return result;
            }
            result = await domainContext.PublishAsync(domainEvent);
            if (result.Result == ActionResult.Failed)
            {
                return result;
            }
            await AppRuntime.Resolve<IEventStore>().UpdateResultAsync(domainEvent, EventResult.Executed);
            this.Version = domainEvent.Version;
            return result;
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
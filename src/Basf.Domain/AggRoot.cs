using System;
using System.Collections.Generic;
using Basf.Domain.Event;
using System.Linq;
using Basf.Domain.Storage;
using System.Threading.Tasks;
using Basf.Data;
using Basf.Messages;

namespace Basf.Domain
{
    public abstract class AggRoot : IAggRoot, IEventSourcing
    {
        private string uniqueId = String.Empty;
        private static string Topic = null;
        private static string AggRootType = null;
        private List<IDomainEvent> uncommittedEvents = new List<IDomainEvent>();
        string IEntity.UniqueId { get { return this.uniqueId; } }
        public int Version { get; private set; }
        public AggRoot(string uniqueId)
        {
            this.uniqueId = uniqueId;
            this.Version = 0;
            Type aggRootType = this.GetType();
            AggRootType = aggRootType.FullName;
            Topic = aggRootType.Name;
        }
        protected async Task<ActionResponse> ApplyChange(IDomainEvent domainEvent)
        {
            Message<IDomainEvent> msg = new Message<IDomainEvent> { CommandId= domainEvent.CommandId, MessageType= "sdfd|", Body= domainEvent };
            domainEvent.AggRootType = AggRootType;
            msg.MessageType = Topic;
            //msg.RoutingKey = this.uniqueId;
            return await AppRuntime.Resolve<IDomainContext>().ApplyChange(msg);
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
                return ActionResponse.Fail((int)DomainError.EventVersionError, String.Format("事件执行异常，聚合根{0}版本Version：{1},最小事件版本Version：{2},当前事件版本Version：{3}",
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
            return String.Format("{{AggRootType:{0},AggRootId:{1}}}", this.GetType().FullName, this.uniqueId);
        }
        private async Task<ActionResponse> HandleEvent(IDomainEvent domainEvent)
        {
            var domainContext = AppRuntime.Resolve<IDomainContext>();
            //var result = domainContext.InvokeHandler(this, domainEvent);
            //if (result.Success == ActionResult.Failed)
            //{
            //    return result;
            //}
            //result = await domainContext.PublishAsync(domainEvent);
            //if (result.Success == ActionResult.Failed)
            //{
            //    return result;
            //}
            await AppRuntime.Resolve<IEventStore>().UpdateResultAsync(domainEvent, EventResult.Executed);
            this.Version = domainEvent.Version;
            return ActionResponse.Success;
        }
    }
    [Serializable]
    public abstract class AggRoot<TAggRootId> : AggRoot
    {
        public TAggRootId UniqueId { get; private set; }
        public AggRoot(TAggRootId uniqueId)
            : base(uniqueId.ToString())
        {
            this.UniqueId = uniqueId;
        }
    }
}
using System;

namespace Basf.Domain.Event
{
    public abstract class DomainEvent<TAggRootId> : IDomainEvent<TAggRootId>
    {
        public string UniqueId { get; private set; }
        public string EventType { get; set; }
        public TAggRootId AggRootId { get; internal set; }
        public string CommandId { get; internal set; }
        public string RoutingKey { get; set; }
        public DateTime Timestamp { get; private set; }
        public int Version { get; internal set; }
        public DomainEvent()
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
        }
    }
}

using System;

namespace Basf.Domain
{
    public interface IDomainEvent<TAggRootId, TEvent> : IDomainEvent<TAggRootId> where TEvent : class
    {
        new TEvent Body { get; set; }
    }
    public interface IDomainEvent<TAggRootId> : IEntity<string>
    {
        TAggRootId AggRootId { get; set; }
        int Version { get; set; }
        DateTime Timestamp { get; }
        object Body { get; set; }
    }
}

using System;

namespace Basf.Domain
{
    public interface IDomainEvent<TKey>
    {
        string UniqueId { get; }
        string CommandId { get; }
        string EventType { get; }
        TKey AggRootId { get; set; }
        int Version { get; set; }
        DateTime Timestamp { get; }
    }
    public interface DomainEvent : IDomainEvent<string>
    {
    }
}

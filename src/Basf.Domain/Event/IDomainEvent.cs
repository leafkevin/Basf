using Basf.Message;
using System;

namespace Basf.Domain.Event
{
    public interface IDomainEvent : IMessage, IComparable, IComparable<IDomainEvent>, IEquatable<IDomainEvent>
    {
        string AggRootType { get; set; }
        string AggRootId { get; set; }
        string CommandId { get; set; }
        int Version { get; set; }
    }
    public interface IDomainEvent<TAggRootId> : IDomainEvent, IComparable<IDomainEvent<TAggRootId>>, IEquatable<IDomainEvent<TAggRootId>>
    {
        new TAggRootId AggRootId { get; set; }
    }
}

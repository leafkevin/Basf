namespace Basf.Domain.Event
{
    public interface IDomainEvent<TAggRootId> : IMessage
    {
        string EventType { get; set; }
        string CommandId { get; }
        TAggRootId AggRootId { get; }
        int Version { get; }
    }
}

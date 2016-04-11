namespace Basf.Domain
{
    public interface IAggregateRoot<TAggRootId> : IEntity<TAggRootId>
    {
        int Version { get; }
    }
}

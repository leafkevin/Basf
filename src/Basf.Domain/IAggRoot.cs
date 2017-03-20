namespace Basf.Domain
{
    public interface IAggRoot : IEntity
    {
        int Version { get; }
    }
    public interface IAggRoot<TAggRootId> : IAggRoot, IEntity<TAggRootId>
    {
        new TAggRootId UniqueId { get; }
    }
}

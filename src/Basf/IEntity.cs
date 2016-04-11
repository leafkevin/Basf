namespace Basf
{
    public interface IEntity
    {
    }
    public interface IEntity<TUniqueId> : IEntity
    {
        TUniqueId UniqueId { get; }
    }
}
